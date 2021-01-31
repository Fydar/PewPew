using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class GameTickProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			foreach (var projectileKvp in view.Lobby.World.Projectiles)
			{
				var projectile = projectileKvp.Value;

				projectile.Position += projectile.Velocity;
				projectile.LifetimeRemaining--;
			}


			var removeKeys = new List<LocalId>();
			foreach (var beamKvp in view.Lobby.World.Beams)
			{
				var beam = beamKvp.Value;

				beam.LifetimeRemaining--;

				if (beam.LifetimeRemaining <= 0)
				{
					removeKeys.Add(beamKvp.Key);
				}
			}
			foreach (var removeKey in removeKeys)
			{
				view.Lobby.World.Beams.Remove(removeKey);
			}


			foreach (var shipKvp in view.Lobby.World.Ships)
			{
				var ship = shipKvp.Value;

				if (ship.BeamsCooldownRemaining > 0)
				{
					ship.BeamsCooldownRemaining--;
				}

				if (ship.BarrageCooldownRemaining > 0)
				{
					ship.BarrageCooldownRemaining--;
				}
			}
		}
	}
}
