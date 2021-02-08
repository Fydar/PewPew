using Newtonsoft.Json;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Model;
using System.IO;

namespace PewPew.WebApp.Server.Services
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
				var stream = new MemoryStream(message.Body.Array, message.Body.Offset, message.Body.Count, false, false);
				var clientCommand = DeserializeClientCommand(stream);

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
