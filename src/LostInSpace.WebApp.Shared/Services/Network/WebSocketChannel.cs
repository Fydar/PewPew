using System;
using System.Buffers;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class WebSocketChannel : INetworkChannel
	{
		public NetworkLogChannel Logging { get; }

		public WebSocket WebSocket { get; private set; }
		public bool IsConnected { get; private set; }

		public event Action OnConnected;
		public event Action<NetworkChannelMessage> OnReceive;

		public WebSocketChannel()
		{
			Logging = new NetworkLogChannel();
		}

		public WebSocketChannel(NetworkLogChannel logging)
		{
			Logging = logging;
		}

		public void UseWebSocket(WebSocket webSocket)
		{
			WebSocket = webSocket;

			IsConnected = true;
			OnConnected?.Invoke();
		}

		public async Task Connect(Uri uri, CancellationToken cancellationToken = default)
		{
			if (WebSocket != null)
			{
				throw new InvalidOperationException($"{nameof(WebSocketChannel)} is already in use.");
			}

			var clientWebSocket = new ClientWebSocket();

			// clientWebSocket.Options.SetRequestHeader("Authorization", "Bearer ######");
			// clientWebSocket.Options.SetRequestHeader("X-EnvironmentUser", Environment.UserDomainName);

			WebSocket = clientWebSocket;

			var connectLog = Logging.Start($"Connect to \"{uri}\"");

			try
			{
				await clientWebSocket.ConnectAsync(uri, cancellationToken);
				OnConnected?.Invoke();
			}
			catch (Exception exception)
			{
				connectLog.End(LogLevel.Error, "Exception thrown during connection", exception: exception);
				return;
			}

			connectLog.End(LogLevel.Information, "Successfully connected");

			IsConnected = true;
		}

		public async Task CloseAsync(CancellationToken cancellationToken = default)
		{
			var trace = Logging.Start("Closing connection");

			try
			{
				await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Scheduled Closure", cancellationToken);
			}
			catch (Exception exception)
			{
				trace.End(LogLevel.Error, "Exception thrown during connection closure", exception: exception);
				return;
			}

			trace.End(LogLevel.Information, "Successfully closed connetion");
		}

		public async Task SendAsync(byte[] message, CancellationToken cancellationToken = default)
		{
			var trace = Logging.Start($"Sending");

			var messageBytes = new ArraySegment<byte>(message);

			try
			{
				await WebSocket.SendAsync(messageBytes, WebSocketMessageType.Binary, true, cancellationToken);
			}
			catch (Exception exception)
			{
				trace
					.PushProperty("Message", Encoding.UTF8.GetString(message))
					.End(LogLevel.Error, "Exception thrown during send", exception);
				return;
			}
			trace
				.PushProperty("Message", Encoding.UTF8.GetString(message))
				.End(LogLevel.Debug, "Sent", null);
		}

		public async Task ListenAsync(CancellationToken cancellationToken = default)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				NetworkLog trace = null;
				WebSocketReceiveResult result;

				byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(8192);
				var bufferSegment = new ArraySegment<byte>(rentedBuffer);
				using var memoryStream = new MemoryStream();

				try
				{
					do
					{
						if (cancellationToken.IsCancellationRequested
							|| (WebSocket.State != WebSocketState.Open
							&& WebSocket.State != WebSocketState.Connecting))
						{
							return;
						}

						result = await WebSocket.ReceiveAsync(bufferSegment, cancellationToken);
						if (trace == null)
						{
							trace = Logging.Start($"Receiving");
						}

						memoryStream.Write(bufferSegment.Array, bufferSegment.Offset, result.Count);
					}
					while (!result.EndOfMessage);
				}
				catch (Exception exception)
				{
					if (trace == null)
					{
						trace = Logging.Start($"Receiving");
					}
					trace.End(LogLevel.Error, "Exception thrown during listen", exception: exception);
					return;
				}
				finally
				{
					ArrayPool<byte>.Shared.Return(rentedBuffer);
				}

				byte[] body = memoryStream.ToArray();
				var asStream = new MemoryStream(body);

				if (!result.CloseStatus.HasValue)
				{
					trace
						.PushProperty("Message", Encoding.UTF8.GetString(body))
						.End(LogLevel.Debug, $"Recieved");

					OnReceive?.Invoke(new NetworkChannelMessage(this, asStream));
				}
				else
				{
					trace
						.End(LogLevel.Information, $"Closed {result.CloseStatus}");
				}
			}
		}
	}
}
