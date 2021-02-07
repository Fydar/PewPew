using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class ProjectileDestroyProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			// view.Lobby.World.Projectiles.Remove(ProjectileId);

			var projectile = view.Lobby.World.Projectiles[ProjectileId];

			projectile.LifetimeRemaining = 0;
		}
	}
}
