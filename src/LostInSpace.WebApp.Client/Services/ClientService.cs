using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.Services.Network;
using LostInSpace.WebApp.Shared.View;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HuskyNet.WebClient.Services
{
	public class ClientService
	{
		private readonly ILogger logger;
		private readonly JsonSerializer serializer;
		private WebSocketChannel webSocketChannel;

		public ClientNetworkedView View { get; private set; }

		public event Action<NetworkedViewProcedure> OnProcedureApplied;

		public ClientService(ILogger<ClientService> logger, NavigationManager navigationManager)
		{
			this.logger = logger;
			serializer = new JsonSerializer();

			string url = navigationManager.BaseUri
				.Replace("http://", "ws://")
				.Replace("https://", "wss://")
				.TrimEnd('/');

			_ = RunAsync($"{url}/api/game");
		}

		public Task SendCommandAsync(ClientCommand command)
		{
			byte[] data = SerializeCommand(command);

			return webSocketChannel.SendAsync(data);
		}

		private async Task RunAsync(string serverUrl, CancellationToken cancellationToken = default)
		{
			var target = new Uri(serverUrl);

			View = new ClientNetworkedView();

			// Client
			logger.LogInformation($"Connecting to {target}");

			try
			{
				webSocketChannel = await WebSocketChannel.ConnectAsync(target, cancellationToken);
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Error whilst connecting");
				return;
			}

			await foreach (var networkEvent in webSocketChannel.ListenAsync(cancellationToken))
			{
				switch (networkEvent)
				{
					case WebSocketBinaryMessageEvent message:
					{
						try
						{
							var procedure = DeserializeProcedure(message.Body);
							procedure.ApplyToView(View);
							OnProcedureApplied?.Invoke(procedure);
						}
						catch (Exception exception)
						{
							logger.LogError(exception, "Error whilst applying procedure to view");
						}
						break;
					}
					case WebSocketExceptionDisconnectEvent disconnectEvent:
					{
						logger.LogError(disconnectEvent.InnerException, "Client connection encountered an exception)");
						break;
					}
					case WebSocketDisconnectEvent:
					{
						logger.LogInformation("Connection with client closed successfully");
						break;
					}
				}
			}

			OnProcedureApplied?.Invoke(null);
		}

		private byte[] SerializeCommand(ClientCommand command)
		{
			var serialized = PackagedModel<ClientCommand>.Serialize(command);

			using var memoryStream = new MemoryStream();
			using var streamWriter = new StreamWriter(memoryStream);
			using var jsonWriter = new JsonTextWriter(streamWriter);

			serializer.Serialize(jsonWriter, serialized);
			jsonWriter.Flush();
			return memoryStream.ToArray();
		}

		private NetworkedViewProcedure DeserializeProcedure(Stream data)
		{
			using var sr = new StreamReader(data);
			using var jr = new JsonTextReader(sr);

			var deserialized = serializer.Deserialize<PackagedModel<NetworkedViewProcedure>>(jr);
			return deserialized.Deserialize();
		}
	}
}
