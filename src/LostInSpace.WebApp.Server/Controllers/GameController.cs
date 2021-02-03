using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Server.Services;
using LostInSpace.WebApp.Shared.Services.Network;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
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

			var socketChannel = new WebSocketChannel();

			socketChannel.Logging.OnComplete += log =>
			{
				var disposables = new List<IDisposable>
				{
					LogContext.PushProperty("Elapsed", log.ElapsedTime),
					LogContext.PushProperty("Result", log.Result)
				};
				foreach (var property in log.Properties)
				{
					disposables.Add(LogContext.PushProperty(property.Key, property.Value));
				}

				if (log.Level >= Shared.Services.Network.LogLevel.Debug)
				{
					logger.Log((LogLevel)(int)log.Level, log.Exception, "MessageLog");
				}

				foreach (var disposable in disposables)
				{
					disposable.Dispose();
				}
			};

			socketChannel.UseWebSocket(currentSocket);
			var clientConnection = new GameClientConnection(LocalId.NewId(), socketChannel)
			{
				CommandProcessor = serverFrontend
			};

			connectionManagerService.Connections.AcceptNewClientConnection(clientConnection);

			logger.Log(LogLevel.Information, "Accepted client WebSocket connection");

			try
			{
				await socketChannel.ListenAsync(cancellationToken);
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Client connection encountered an exception");
			}

			logger.Log(LogLevel.Information, "Closed client WebSocket connection");

			connectionManagerService.Connections.RemoveClientConnection(clientConnection);
		}
	}
}
