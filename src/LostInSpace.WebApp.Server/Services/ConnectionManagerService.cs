using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Model;
using Newtonsoft.Json;
using System.IO;

namespace LostInSpace.WebApp.Server.Services
{
	public class ConnectionManagerService
	{
		private readonly JsonSerializer serializer;

		public GameClientConnectionPortal Connections { get; }

		public ConnectionManagerService(ServerPortal serverFrontend)
		{
			serializer = new JsonSerializer();

			Connections = new GameClientConnectionPortal();
			Connections.OnConnect += connection =>
			{
				connection.CommandProcessor = serverFrontend.Frontend;
				connection.CommandProcessor.AddPlayer(connection);
			};
			Connections.OnDisconnect += connection =>
			{
				connection.CommandProcessor.RemovePlayer(connection);
			};
			Connections.OnMessageRecieved += (connection, message) =>
			{
				var clientCommand = DeserializeClientCommand(message.Body);

				connection.CommandProcessor.RecieveCommandFromPlayer(connection, clientCommand);
			};
		}

		private ClientCommand DeserializeClientCommand(Stream data)
		{
			using var sr = new StreamReader(data);
			using var jr = new JsonTextReader(sr);
			var deserialized = serializer.Deserialize<PackagedModel<ClientCommand>>(jr);
			return deserialized.Deserialize();
		}
	}
}
