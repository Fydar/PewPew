using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerUpdateClassProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public string ShipClass { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var player = view.Lobby.Players[Identifier];

			player.ShipClass = ShipClass;

			// Prevent multiple players from having capital ships
			if (ShipClass == ShipTypes.Capital)
			{
				foreach (var otherPlayer in view.Lobby.Players)
				{
					if (otherPlayer.Key != Identifier
						&& otherPlayer.Value.ShipClass == ShipTypes.Capital
						&& otherPlayer.Value.TeamId == player.TeamId)
					{
						otherPlayer.Value.ShipClass = ShipTypes.Scout;
					}
				}
			}
		}
	}
}
