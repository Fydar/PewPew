using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Server.Services
{
	public class GameServerWorker
	{
		private readonly JsonSerializer serializer;

		public InstanceViewCommandProcessor CommandProcessor { get; }
		public GameClientConnectionPortal Portal { get; }
		public NetworkedView View { get; }

		public GameServerWorker()
		{
			serializer = new JsonSerializer();

			Portal = new GameClientConnectionPortal();
			View = new ServerNetworkedView();
			CommandProcessor = new InstanceViewCommandProcessor(View);

			View.Lobby = new LobbyView();

			Portal.OnConnect += connection =>
			{
				var procedures = CommandProcessor.HandlePlayerConnect(connection);
				ApplyViewProcedures(procedures, connection);
			};
			Portal.OnDisconnect += connection =>
			{
				var procedures = CommandProcessor.HandlePlayerDisconnect(connection);
				ApplyViewProcedures(procedures, connection);
			};
			Portal.OnMessageRecieved += (connection, message) =>
			{
				var command = DeserializeClientCommand(message.Content);
				var procedures = CommandProcessor.HandleRecieveCommand(connection, command);
				ApplyViewProcedures(procedures, connection);
			};

			_ = GameTickerWorker();
		}

		private async Task GameTickerWorker()
		{
			while (true)
			{
				await Task.Delay(1000);

				var procedures = CommandProcessor.HandleGameTick();
				ApplyViewProcedures(procedures, null);
			}
		}

		private void ApplyViewProcedures(IEnumerable<ScopedNetworkedViewProcedure> scopedProcedures, GameClientConnection sender = null)
		{
			foreach (var scopedProcedure in scopedProcedures)
			{
				if (sender == null && scopedProcedure.Scope == ProcedureScope.Reply)
				{
					throw new InvalidOperationException("Cannot reply when procedure is being applied from game tick");
				}

				try
				{
					scopedProcedure.Procedure.ApplyToView(View);
				}
				catch(Exception exception)
				{
					Console.WriteLine(exception);
					continue;
				}

				byte[] serialized = SerializeProcedure(scopedProcedure.Procedure);

				for (int i = Portal.Connections.Count - 1; i >= 0; i--)
				{
					var gameClient = Portal.Connections[i];
					// If we are forwarding; don't send to the original sender.
					if (scopedProcedure.Scope == ProcedureScope.Forward
						&& gameClient == sender)
					{
						continue;
					}

					// If we are replying; only send to the original sender.
					if (scopedProcedure.Scope == ProcedureScope.Reply
						&& gameClient != sender)
					{
						continue;
					}

					_ = gameClient.Connection.SendAsync(serialized);
				}
			}
		}

		private ClientCommand DeserializeClientCommand(Stream data)
		{
			using var sr = new StreamReader(data);
			using var jr = new JsonTextReader(sr);
			var deserialized = serializer.Deserialize<PackagedModel<ClientCommand>>(jr);
			return deserialized.Deserialize();
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
