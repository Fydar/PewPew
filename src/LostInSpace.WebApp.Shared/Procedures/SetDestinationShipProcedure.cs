using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class SetDestinationShipProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public LocalId Target { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var ship = view.Lobby.World.Ships[Identifier];

			ship.Action = ShipAction.MoveToShip;
			ship.ActionTarget = Target;
		}
	}
}
