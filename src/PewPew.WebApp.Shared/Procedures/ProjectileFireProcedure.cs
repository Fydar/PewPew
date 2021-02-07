using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class ProjectileFireProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }
		public Projectile Projectile { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var owner = view.Lobby.World.Ships[Projectile.Owner];
			owner.BarrageCooldownRemaining = owner.BarrageCoodownWait;

			view.Lobby.World.Projectiles.Add(ProjectileId, Projectile);
		}
	}
}
