﻿namespace LostInSpace.WebApp.Shared.Model
{
	public class LobbyStatus
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public int CurrentPlayers { get; set; }
		public int MaxPlayers { get; set; }
	}
}
