using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class SetDestinationShipProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public LocalId Target { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}

			var ship = view.Lobby.World.Ships[Identifier];

			ship.Action = ShipAction.MoveToShip;
			ship.ActionTarget = Target;
		}
	}
}
