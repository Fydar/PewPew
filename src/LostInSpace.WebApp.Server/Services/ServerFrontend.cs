using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.Procedures;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Server.Services
{
	public class ServerFrontend : ICommandProcessor
	{
		private readonly ServerPortal serverPortal;
		private readonly JsonSerializer serializer;
		private readonly List<GameClientConnection> updateSubscribers;

		private bool isDirty = false;
		private byte[] lastUpdate;

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

				if (isDirty)
				{
					isDirty = false;
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
			isDirty = true;
			updateSubscribers.Add(connection);

			if (lastUpdate != null)
			{
				_ = connection.SendAsync(lastUpdate);
			}
		}

		public void RemovePlayer(GameClientConnection connection)
		{
			isDirty = true;
			updateSubscribers.Remove(connection);
		}

		public void RecieveCommandFromPlayer(GameClientConnection connection, ClientCommand clientCommand)
		{
			isDirty = true;

			switch (clientCommand)
			{
				case FrontendCreateLobbyCommand:
				{
					isDirty = true;

					var lobby = serverPortal.CreateLobby();

					connection.CommandProcessor.RemovePlayer(connection);
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
						isDirty = true;

						connection.CommandProcessor.RemovePlayer(connection);
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
