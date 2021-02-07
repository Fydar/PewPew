using PewPew.WebApp.Shared.Model;
using Newtonsoft.Json;

namespace PewPew.WebApp.Shared.View
{
	public class Projectile
	{
		public LocalId Owner { get; set; }
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public Vector2 Velocity { get; set; }
		public int LifetimeRemaining { get; set; } = 20;
		public int Damage { get; set; } = 20;

		[JsonIgnore]
		public bool IsDestroyed => LifetimeRemaining <= 0;
	}
}
