using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.Services.Network;
using LostInSpace.WebApp.Shared.View;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace HuskyNet.WebClient.Services
{
	public class ClientService
	{
		private readonly ILogger logger;
		private readonly JsonSerializer serializer;
		private WebSocketChannel serverConnection;

		public ClientNetworkedView View { get; private set; }

		public event Action<NetworkedViewProcedure> OnProcedureApplied;

		public ClientService(ILogger<ClientService> logger)
		{
			this.logger = logger;

			serializer = new JsonSerializer();

			_ = ConnectAsync("wss://localhost:5001");
		}

		public async Task ConnectAsync(string serverUrl)
		{
			try
			{
				var target = new Uri($"{serverUrl}/api/game");

				var clientView = new ClientNetworkedView();

				// Client
				serverConnection = new WebSocketChannel();
				serverConnection.Logging.OnComplete += log =>
				{
					if (log.Level >= LostInSpace.WebApp.Shared.Services.Network.LogLevel.Information)
					{
						logger.LogInformation(log.ToString());
					}
				};
				serverConnection.OnReceive += message =>
				{
					try
					{
						var procedure = DeserializeProcedure(message.Content);
						procedure.ApplyToView(clientView);
						OnProcedureApplied?.Invoke(procedure);
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
				};

				logger.LogInformation($"Connecting to {target}");
				await serverConnection.Connect(target);

				if (serverConnection.IsConnected)
				{
					logger.LogInformation($"Connected to {target}");
				}
				else
				{
					logger.LogInformation($"Failed to connect to {target}");
				}
				OnProcedureApplied?.Invoke(null);

				View = clientView;
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}

		public Task SendCommandAsync(ClientCommand command)
		{
			byte[] data = SerializeCommand(command);

			return serverConnection.SendAsync(data);
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
