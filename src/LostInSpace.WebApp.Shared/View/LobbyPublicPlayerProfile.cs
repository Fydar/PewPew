namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// A public representation of a player connected to the lobby.
	/// </summary>
	public class LobbyPublicPlayerProfile
	{
		public int TeamId { get; set; }
		public string DisplayName { get; set; }
		public string ShipClass { get; set; } = ShipTypes.Scout;
	}
}
