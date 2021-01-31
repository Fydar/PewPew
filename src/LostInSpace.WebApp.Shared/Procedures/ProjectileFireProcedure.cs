using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class ProjectileFireProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }
		public Projectile Projectile { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var owner = view.Lobby.World.Ships[Projectile.Owner];
			owner.BeamsCooldownRemaining = owner.BeamsCooldownWait;

			view.Lobby.World.Projectiles.Add(ProjectileId, Projectile);
		}
	}
}
