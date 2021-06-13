using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class ProjectileDestroyProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}
			// view.Lobby.World.Projectiles.Remove(ProjectileId);

			var projectile = view.Lobby.World.Projectiles[ProjectileId];

			projectile.LifetimeRemaining = 0;
		}
	}
}
