using LostInSpace.WebApp.Server.Controllers;
using LostInSpace.WebApp.Server.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HuskyNet.Instance.Server.Services
{
	public class ServerPortal
	{
		private readonly ConcurrentDictionary<string, ServerLobby> lobbies;

		public ServerFrontend Frontend { get; set; }

		public ServerPortal()
		{
			lobbies = new ConcurrentDictionary<string, ServerLobby>();
			Frontend = new ServerFrontend(this);
		}

		public bool TryGetLobby(string lobbyKey, out ServerLobby lobby)
		{
			return lobbies.TryGetValue(lobbyKey, out lobby);
		}

		public ServerLobby CreateLobby()
		{
			string lobbyKey = LobbyIdGenerator.GenerateLobbyKey();

			var lobby = new ServerLobby(Frontend, lobbyKey);

			if (lobbies.TryAdd(lobbyKey, lobby))
			{
				return lobby;
			}
			else
			{
				throw new InvalidOperationException($"Failed to create lobby \"{lobbyKey}\"");
			}
		}

		public IEnumerable<ServerLobby> AllLobbies()
		{
			return lobbies.Values;
		}
	}
}
