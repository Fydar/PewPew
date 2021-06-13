using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class UpdatePositionProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }

		public Vector2 Position { get; set; }
		public float Rotation { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}

			var ship = view.Lobby.World.Ships[Identifier];

			ship.Position = Position;
			ship.Rotation = Rotation;
		}
	}
}
