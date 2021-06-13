using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class ProjectileFireProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }
		public Projectile? Projectile { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (Projectile == null)
			{
				throw new InvalidOperationException("Cannot apply procedure as it is malformed.");
			}

			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}

			var owner = view.Lobby.World.Ships[Projectile.Owner];
			owner.BarrageCooldownRemaining = owner.BarrageCoodownWait;

			view.Lobby.World.Projectiles.Add(ProjectileId, Projectile);
		}
	}
}
