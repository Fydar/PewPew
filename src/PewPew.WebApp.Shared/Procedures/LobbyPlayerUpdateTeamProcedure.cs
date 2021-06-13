using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerUpdateTeamProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public int TeamId { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid lobby.");
			}

			var player = view.Lobby.Players[Identifier];

			player.TeamId = TeamId;
		}
	}
}
