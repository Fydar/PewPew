using Newtonsoft.Json;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.Procedures;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PewPew.WebApp.Server.Services
{
	public class ServerFrontend : ICommandProcessor
	{
		private readonly ServerPortal serverPortal;
		private readonly JsonSerializer serializer;
		private readonly List<GameClientConnection> updateSubscribers;

		public bool IsDirty { get; set; } = false;
		private byte[]? lastUpdate;

		public ServerFrontend(
			ServerPortal serverPortal)
		{
			this.serverPortal = serverPortal;

			serializer = new JsonSerializer();
			updateSubscribers = new List<GameClientConnection>();

			_ = RunFrontendWorkerAsync();
		}

		private async Task RunFrontendWorkerAsync(CancellationToken cancellationToken = default)
		{
			while (true)
			{
				await Task.Delay(1000 * 1, cancellationToken);

				if (IsDirty)
				{
					IsDirty = false;
					var lobbies = serverPortal.AllLobbies();
					var lobbyStatuses = new List<LobbyStatus>(lobbies.Select(l => l.Status));

					var updateProcedure = new FrontendListUpdateProcedure()
					{
						Lobbies = lobbyStatuses
					};
					lastUpdate = SerializeProcedure(updateProcedure);

					foreach (var subscriber in updateSubscribers)
					{
						_ = subscriber.SendAsync(lastUpdate, cancellationToken);
					}
				}
			}
		}

		public void AddPlayer(GameClientConnection connection)
		{
			IsDirty = true;
			updateSubscribers.Add(connection);

			if (lastUpdate != null)
			{
				_ = connection.SendAsync(lastUpdate);
			}
		}

		public void RemovePlayer(GameClientConnection connection)
		{
			IsDirty = true;
			updateSubscribers.Remove(connection);
		}

		public void RecieveCommandFromPlayer(GameClientConnection connection, ClientCommand clientCommand)
		{
			IsDirty = true;

			switch (clientCommand)
			{
				case FrontendCreateLobbyCommand:
				{
					IsDirty = true;

					var lobby = serverPortal.CreateLobby();

					connection.CommandProcessor?.RemovePlayer(connection);
					connection.CommandProcessor = lobby;
					connection.CommandProcessor.AddPlayer(connection);

					break;
				}
				case FrontendJoinLobbyCommand command:
				{
					if (command.LobbyKey == null)
					{
						break;
					}

					if (serverPortal.TryGetLobby(command.LobbyKey, out var lobby))
					{
						IsDirty = true;

						connection.CommandProcessor?.RemovePlayer(connection);
						connection.CommandProcessor = lobby;
						connection.CommandProcessor.AddPlayer(connection);
					}
					else
					{

					}
					break;
				}
			}
		}

		private byte[] SerializeProcedure(NetworkedViewProcedure viewProcedure)
		{
			var serialized = PackagedModel<NetworkedViewProcedure>.Serialize(viewProcedure);

			using var ms = new MemoryStream();
			using (var sr = new StreamWriter(ms))
			using (var jw = new JsonTextWriter(sr))
			{
				serializer.Serialize(jw, serialized);
			}
			return ms.ToArray();
		}
	}
}
