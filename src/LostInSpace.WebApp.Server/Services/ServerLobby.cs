using LostInSpace.WebApp.Server.Game;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Server.Services
{
	public class ServerLobby : ICommandProcessor
	{
		private readonly Mutex viewMutex;
		private readonly JsonSerializer serializer;
		private readonly InstanceViewCommandProcessor commandProcessor;
		private readonly ServerNetworkedView view;
		private readonly List<GameClientConnection> players;
		private readonly ServerFrontend serverFrontend;

		public LobbyStatus Status { get; }

		public ServerLobby(ServerFrontend serverFrontend, string lobbyKey)
		{
			Status = new LobbyStatus()
			{
				Key = lobbyKey,
				CurrentPlayers = 0,
				MaxPlayers = 60,
				Name = lobbyKey
			};

			viewMutex = new Mutex(false);
			serializer = new JsonSerializer();
			players = new List<GameClientConnection>();

			view = new ServerNetworkedView
			{
				Lobby = new LobbyView
				{
					LobbyKey = lobbyKey
				}
			};
			commandProcessor = new InstanceViewCommandProcessor(view);

			_ = GameTickerWorker();
			this.serverFrontend = serverFrontend;
		}

		public void AddPlayer(GameClientConnection connection)
		{
			viewMutex.WaitOne();
			players.Add(connection);
			Status.CurrentPlayers = players.Count;

			var procedures = commandProcessor.HandlePlayerConnect(connection).ToList();
			ApplyViewProcedures(procedures, connection);
			viewMutex.ReleaseMutex();
		}

		public void RemovePlayer(GameClientConnection connection)
		{
			viewMutex.WaitOne();
			players.Remove(connection);
			Status.CurrentPlayers = players.Count;

			var procedures = commandProcessor.HandlePlayerDisconnect(connection).ToList();
			ApplyViewProcedures(procedures, connection);
			viewMutex.ReleaseMutex();
		}

		public void RecieveCommandFromPlayer(GameClientConnection connection, ClientCommand clientCommand)
		{
			switch (clientCommand)
			{
				case LobbyLeaveCommand:
				{
					if (connection.CommandProcessor == this)
					{
						RemovePlayer(connection);

						connection.CommandProcessor = serverFrontend;
						AddPlayer(connection);
					}
					break;
				}
				default:
					viewMutex.WaitOne();
					var procedures = commandProcessor.HandleRecieveCommand(connection, clientCommand).ToList();
					ApplyViewProcedures(procedures, connection);
					viewMutex.ReleaseMutex();
					break;
			}
		}

		private async Task GameTickerWorker()
		{
			while (true)
			{
				await Task.Delay(1000 / 4);

				viewMutex.WaitOne();
				var procedures = commandProcessor.HandleGameTick().ToList();
				ApplyViewProcedures(procedures, null);
				viewMutex.ReleaseMutex();
			}
		}

		private void ApplyViewProcedures(IReadOnlyList<ScopedNetworkedViewProcedure> scopedProcedures, GameClientConnection sender = null)
		{
			for (int p = 0; p < scopedProcedures.Count; p++)
			{
				var scopedProcedure = scopedProcedures[p];
				if (sender == null && scopedProcedure.Scope == ProcedureScope.Reply)
				{
					throw new InvalidOperationException("Cannot reply when procedure is being applied from game tick");
				}

				try
				{
					scopedProcedure.Procedure.ApplyToView(view);
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
					continue;
				}

				byte[] serialized = SerializeProcedure(scopedProcedure.Procedure);

				foreach (var player in players)
				{
					// If we are forwarding; don't send to the original sender.
					if (scopedProcedure.Scope == ProcedureScope.Forward
						&& player == sender)
					{
						continue;
					}

					// If we are replying; only send to the original sender.
					if (scopedProcedure.Scope == ProcedureScope.Reply
						&& player != sender)
					{
						continue;
					}

					if (scopedProcedure.Procedure is LobbyPlayerKickProcedure kickProcedure)
					{
						if (kickProcedure.Identifier == player.Identifier)
						{
							_ = player.Channel.CloseAsync();
							continue;
						}
					}

					_ = player.Channel.SendAsync(serialized);
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
