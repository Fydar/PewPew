using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class ProjectileDestroyProcedure : NetworkedViewProcedure
	{
		public LocalId ProjectileId { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby.World.Projectiles.Remove(ProjectileId);
		}
	}
}
