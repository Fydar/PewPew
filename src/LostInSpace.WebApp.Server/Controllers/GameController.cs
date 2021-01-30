using Husky.Game.Shared.Model;
using HuskyNet.Instance.Server.Services;
using LostInSpace.WebApp.Server.Services;
using LostInSpace.WebApp.Shared.Services.Network;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class GameController : ControllerBase
	{
		private readonly ILogger<GameController> logger;
		private readonly InstanceManagerService instanceManagerService;

		public GameController(ILogger<GameController> logger, InstanceManagerService instanceManagerService)
		{
			this.logger = logger;
			this.instanceManagerService = instanceManagerService;
		}

		[HttpGet]
		public async Task Get()
		{
			var context = ControllerContext.HttpContext;
			bool isSocketRequest = context.WebSockets.IsWebSocketRequest;
			if (!isSocketRequest)
			{
				context.Response.StatusCode = 400;
				return;
			}

			var ct = context.RequestAborted;
			var currentSocket = await context.WebSockets.AcceptWebSocketAsync();
			string socketId = Guid.NewGuid().ToString();

			var socketChannel = new WebSocketChannel();

			socketChannel.Logging.OnComplete += log =>
			{
				// LogContext.PushProperty("Name", log.Name);
				var disposables = new List<IDisposable>
				{
					LogContext.PushProperty("Elapsed", log.ElapsedTime)
				};
				foreach (var property in log.Properties)
				{
					disposables.Add(LogContext.PushProperty(property.Key, property.Value));
				}
				disposables.Add(LogContext.PushProperty("Result", log.Result));

				if (log.Level > Shared.Services.Network.LogLevel.Debug)
				{
					logger.Log(ConvertToLogLevel(log.Level), log.Exception, "MessageLog");
				}

				foreach (var disposable in disposables)
				{
					disposable.Dispose();
				}
			};

			var connection = new GameClientConnection(LocalId.NewId(), socketChannel);

			try
			{
				socketChannel.UseWebSocket(currentSocket);
				instanceManagerService.Worker.Portal.AcceptNewClientConnection(connection);

				logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, "Accepting client WebSocket connection");
				socketChannel.StartListening();

				while (true)
				{
					if (ct.IsCancellationRequested
						|| (socketChannel.WebSocket.State != WebSocketState.Open
						 && socketChannel.WebSocket.State != WebSocketState.Connecting))
					{
						break;
					}
					await Task.Delay(50);
				}
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Client connection encountered an exception");
			}
			finally
			{
				instanceManagerService.Worker.Portal.RemoveClientConnection(connection);

				if (socketChannel.WebSocket.State != WebSocketState.Open
					 && socketChannel.WebSocket.State != WebSocketState.Connecting)
				{
					try
					{
						await currentSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
					}
					catch (Exception closeException)
					{
						logger.LogWarning(closeException, "Client connection encountered an exception whilst trying to close.");
					}
				}

				currentSocket.Dispose();
			}
		}

		private static Microsoft.Extensions.Logging.LogLevel ConvertToLogLevel(Shared.Services.Network.LogLevel logLevel)
		{
			switch (logLevel)
			{
				case Shared.Services.Network.LogLevel.Trace:
					return Microsoft.Extensions.Logging.LogLevel.Trace;

				case Shared.Services.Network.LogLevel.Debug:
					return Microsoft.Extensions.Logging.LogLevel.Debug;

				case Shared.Services.Network.LogLevel.Information:
					return Microsoft.Extensions.Logging.LogLevel.Information;

				case Shared.Services.Network.LogLevel.Warning:
					return Microsoft.Extensions.Logging.LogLevel.Warning;

				case Shared.Services.Network.LogLevel.Error:
					return Microsoft.Extensions.Logging.LogLevel.Error;

				case Shared.Services.Network.LogLevel.Critical:
					return Microsoft.Extensions.Logging.LogLevel.Critical;

				default:
				case Shared.Services.Network.LogLevel.None:
					return Microsoft.Extensions.Logging.LogLevel.None;
			}
		}
	}
}
