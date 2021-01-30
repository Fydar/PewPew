using LostInSpace.WebApp.Shared.Services.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostInSpace.WebApp.Server.Services
{
	public class GameClientConnectionPortal
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<GameClientConnection> connections;

		public IReadOnlyList<GameClientConnection> Connections => connections;

		public event Action<GameClientConnection> OnConnect;
		public event Action<GameClientConnection> OnDisconnect;
		public event Action<GameClientConnection, NetworkChannelMessage> OnMessageRecieved;

		public GameClientConnectionPortal()
		{
			connections = new List<GameClientConnection>();
		}

		public void AcceptNewClientConnection(GameClientConnection clientConnection)
		{
			void HandleOnMessageRecieved(NetworkChannelMessage message)
			{
				OnMessageRecieved?.Invoke(clientConnection, message);
			}

			connections.Add(clientConnection);
			OnConnect?.Invoke(clientConnection);

			clientConnection.Connection.OnReceive += HandleOnMessageRecieved;
		}

		public void RemoveClientConnection(GameClientConnection clientConnection)
		{
			if (connections.Remove(clientConnection))
			{
				OnDisconnect?.Invoke(clientConnection);
				// clientConnection.Connection.OnReceive -= HandleOnMessageRecieved;
			}
		}
	}
}
