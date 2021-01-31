using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.View
{
	public class Projectile
	{
		public LocalId Owner { get; set; }
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public Vector2 Velocity { get; set; }
		public int LifetimeRemaining { get; set; } = 20;
	}
}
