using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Server.Services;
using LostInSpace.WebApp.Shared.Services.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.IO;
using System.Threading.Tasks;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace LostInSpace.WebApp.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		private readonly ILogger logger;
		private readonly ConnectionManagerService connectionManagerService;
		public ServerFrontend serverFrontend;

		public GameController(
			ILogger<GameController> logger,
			ConnectionManagerService connectionManagerService)
		{
			this.logger = logger;
			this.connectionManagerService = connectionManagerService;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status101SwitchingProtocols)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task Get()
		{
			var context = ControllerContext.HttpContext;
			bool isSocketRequest = context.WebSockets.IsWebSocketRequest;
			if (!isSocketRequest)
			{
				context.Response.StatusCode = 400;
				return;
			}

			var cancellationToken = context.RequestAborted;
			using var currentSocket = await context.WebSockets.AcceptWebSocketAsync();

			var socketChannel = WebSocketChannel.ContinueFrom(currentSocket);
			var clientConnection = new GameClientConnection(LocalId.NewId(), socketChannel)
			{
				CommandProcessor = serverFrontend
			};

			connectionManagerService.Connections.AcceptNewClientConnection(clientConnection);
			logger.Log(LogLevel.Information, "Accepted client WebSocket connection");

			await foreach (var networkEvent in socketChannel.ListenAsync(cancellationToken))
			{
				switch (networkEvent)
				{
					case WebSocketBinaryMessageEvent message:
					{
						if (logger.IsEnabled(LogLevel.Debug))
						{
							using (LogContext.PushProperty("Message", LogStream(message.Body)))
							using (LogContext.PushProperty("Duration", message.Elapsed))
							{
								logger.Log(LogLevel.Debug, "MessageLog");
							}
						}

						connectionManagerService.Connections.HandleOnMessageRecieved(clientConnection, message);

						break;
					}
					case WebSocketExceptionDisconnectEvent disconnectEvent:
					{
						logger.LogError(disconnectEvent.InnerException, "Client connection encountered an exception");
						break;
					}
					case WebSocketDisconnectEvent disconnectEvent:
					{
						break;
					}
				}
			}

			connectionManagerService.Connections.RemoveClientConnection(clientConnection);
			logger.Log(LogLevel.Information, "Closed client WebSocket connection");
		}

		private static string LogStream(Stream stream)
		{
			long originalPosition = stream.Position;

			string logMessage;
			var sr = new StreamReader(stream);
			{
				logMessage = sr.ReadToEnd();
			}
			stream.Position = originalPosition;

			return logMessage;
		}
	}
}
