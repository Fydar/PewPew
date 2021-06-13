using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;
using System.Collections.Generic;

namespace PewPew.WebApp.Shared.Procedures
{
	public class GameTickProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}

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
