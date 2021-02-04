using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class LobbyPlayerUpdateNameProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public string DisplayName { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var player = view.Lobby.Players[Identifier];

			player.DisplayName = DisplayName;
		}
	}
}
