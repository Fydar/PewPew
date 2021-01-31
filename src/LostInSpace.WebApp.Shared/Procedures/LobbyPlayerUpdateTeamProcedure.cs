using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class LobbyPlayerUpdateTeamProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public int TeamId { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var player = view.Lobby.Players[Identifier];

			player.TeamId = TeamId;
		}
	}
}
