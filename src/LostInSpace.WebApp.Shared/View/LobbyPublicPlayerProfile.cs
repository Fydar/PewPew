using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// A public representation of a player connected to the lobby.
	/// </summary>
	public class LobbyPublicPlayerProfile
	{
		public LocalId Identifier { get; set; }

		public string DisplayName { get; set; }
	}
}
