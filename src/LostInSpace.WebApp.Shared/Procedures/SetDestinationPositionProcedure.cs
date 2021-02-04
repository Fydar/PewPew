using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class SetDestinationPositionProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public Vector2 Position { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var ship = view.Lobby.World.Ships[Identifier];

			ship.Action = ShipAction.MoveToPosition;
			ship.ActionPosition = Position;
		}
	}
}
