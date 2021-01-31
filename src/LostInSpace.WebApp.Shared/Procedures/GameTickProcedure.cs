using LostInSpace.WebApp.Shared.View;

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

			foreach (var shipKvp in view.Lobby.World.Ships)
			{
				var ship = shipKvp.Value;

				if (ship.CooldownRemaining > 0)
				{
					ship.CooldownRemaining--;
				}
			}
		}
	}
}
