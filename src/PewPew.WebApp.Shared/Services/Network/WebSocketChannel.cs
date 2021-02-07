using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PewPew.WebApp.Shared.Services.Network
{
	public class WebSocketChannel : INetworkChannel
	{
		public WebSocket WebSocket { get; private set; }

		private WebSocketChannel(WebSocket webSocket)
		{
			WebSocket = webSocket;
		}

		public static async Task<WebSocketChannel> ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
		{
			var clientWebSocket = new ClientWebSocket();

			await clientWebSocket.ConnectAsync(uri, cancellationToken);

			var webSocketChannel = new WebSocketChannel(clientWebSocket);

			return webSocketChannel;
		}

		public static WebSocketChannel ContinueFrom(WebSocket webSocket)
		{
			var webSocketChannel = new WebSocketChannel(webSocket);

			return webSocketChannel;
		}

		public async IAsyncEnumerable<IWebSocketEvent> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					yield break;
				}

				byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(8192);
				var bufferSegment = new ArraySegment<byte>(rentedBuffer);

				DateTimeOffset? startTime = null;

				WebSocketReceiveResult result = null;
				do
				{
					if (cancellationToken.IsCancellationRequested)
					{
						yield break;
					}

					if (WebSocket.State != WebSocketState.Open
						&& WebSocket.State != WebSocketState.Connecting)
					{
						yield return new WebSocketDisconnectEvent()
						{
							CloseStatus = WebSocket.CloseStatus ?? WebSocketCloseStatus.Empty
						};
						ArrayPool<byte>.Shared.Return(rentedBuffer);
						yield break;
					}

					Exception innerException = null;

					try
					{
						result = await WebSocket.ReceiveAsync(bufferSegment, cancellationToken);
					}
					catch (Exception exception)
					{
						innerException = exception;
					}
					startTime ??= DateTimeOffset.UtcNow;

					if (innerException != null)
					{
						yield return new WebSocketExceptionDisconnectEvent()
						{
							StartTime = startTime.Value,
							EndTime = DateTimeOffset.UtcNow,

							CloseStatus = result?.CloseStatus ?? WebSocketCloseStatus.Empty,
							InnerException = innerException
						};

						ArrayPool<byte>.Shared.Return(rentedBuffer);
						yield break;
					}

					if (!result.EndOfMessage)
					{
						byte[] newBuffer = ArrayPool<byte>.Shared.Rent(rentedBuffer.Length * 2);
						rentedBuffer.CopyTo(newBuffer, 0);

						bufferSegment = new ArraySegment<byte>(newBuffer, rentedBuffer.Length,
							newBuffer.Length - rentedBuffer.Length);

						ArrayPool<byte>.Shared.Return(rentedBuffer);
						rentedBuffer = newBuffer;
					}
				}
				while (!result.EndOfMessage);

				if (result.CloseStatus.HasValue)
				{
					ArrayPool<byte>.Shared.Return(rentedBuffer);

					yield return new WebSocketDisconnectEvent()
					{
						StartTime = startTime ?? DateTimeOffset.UtcNow,
						EndTime = DateTimeOffset.UtcNow,
						CloseStatus = result.CloseStatus.Value,
					};
					yield break;
				}
				else
				{
					var wholeBuffer = new ArraySegment<byte>(rentedBuffer, 0, result.Count);

					yield return new WebSocketBinaryMessageEvent()
					{
						StartTime = startTime ?? DateTimeOffset.UtcNow,
						EndTime = DateTimeOffset.UtcNow,

						Body = wholeBuffer,
						rentedArray = rentedBuffer
					};
				}
			}
		}

		public async Task CloseAsync(CancellationToken cancellationToken = default)
		{
			await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Scheduled Closure", cancellationToken);
		}

		public async Task SendAsync(byte[] message, CancellationToken cancellationToken = default)
		{
			var messageBytes = new ArraySegment<byte>(message);

			await WebSocket.SendAsync(messageBytes, WebSocketMessageType.Binary, true, cancellationToken);
		}
	}
}
