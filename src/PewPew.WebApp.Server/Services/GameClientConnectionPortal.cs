using PewPew.WebApp.Shared.Services.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PewPew.WebApp.Server.Services
{
	public class GameClientConnectionPortal
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<GameClientConnection> connections;

		public event Action<GameClientConnection>? OnConnect;
		public event Action<GameClientConnection>? OnDisconnect;
		public event Action<GameClientConnection, WebSocketBinaryMessageEvent>? OnMessageRecieved;

		public IReadOnlyList<GameClientConnection> Connections => connections;

		public GameClientConnectionPortal()
		{
			connections = new List<GameClientConnection>();
		}

		public void AcceptNewClientConnection(GameClientConnection clientConnection)
		{
			connections.Add(clientConnection);
			OnConnect?.Invoke(clientConnection);
		}

		public void RemoveClientConnection(GameClientConnection clientConnection)
		{
			if (connections.Remove(clientConnection))
			{
				OnDisconnect?.Invoke(clientConnection);
			}
		}

		public void HandleOnMessageRecieved(GameClientConnection clientConnection, WebSocketBinaryMessageEvent message)
		{
			OnMessageRecieved?.Invoke(clientConnection, message);
		}
	}
}
