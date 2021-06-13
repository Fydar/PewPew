using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerUpdateNameProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public string DisplayName { get; set; } = string.Empty;

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid lobby.");
			}

			var player = view.Lobby.Players[Identifier];

			player.DisplayName = DisplayName;
		}
	}
}
