using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class ShipDamageProcedure : NetworkedViewProcedure
	{
		public LocalId Source { get; set; }
		public LocalId Target { get; set; }
		public int Damage { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (!(view.Lobby.World?.Ships.ContainsKey(Target) ?? false))
			{
				return;
			}

			var clientNetworkedView = view as ClientNetworkedView;

			var ship = view.Lobby.World.Ships[Target];

			if (ship.IsDestroyed)
			{
				return;
			}

			ship.Health -= Damage;

			if (clientNetworkedView != null && Source == clientNetworkedView.Client.ClientId)
			{
				var targetPlayer = view.Lobby.Players[Target];

				view.Lobby.World.BattleLog.Add($"You hit {targetPlayer} for {Damage} damage!");
			}

			if (ship.IsDestroyed)
			{
				var targetPlayer = view.Lobby.Players[Target];

				if (clientNetworkedView != null && Source == clientNetworkedView.Client.ClientId)
				{
					view.Lobby.World.BattleLog.Add($"You destroyed {targetPlayer.DisplayName}!");
				}
				else
				{
					var sourcePlayer = view.Lobby.Players[Source];

					view.Lobby.World.BattleLog.Add($"{sourcePlayer.DisplayName} destroyed {targetPlayer.DisplayName}!");
				}
			}
		}
	}
}
