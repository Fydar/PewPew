namespace PewPew.WebApp.Shared.Model
{
	public class LobbyStatus
	{
		public string Key { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public int CurrentPlayers { get; set; }
		public int MaxPlayers { get; set; }
	}
}
