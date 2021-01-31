using Husky.Game.Shared.Model;
using Newtonsoft.Json;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// Represents a playable character controlled by a player.
	/// </summary>
	public class GameplayShip
	{
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public int Health { get; set; } = 100;

		[JsonIgnore] public bool CanShoot => BeamsCooldownRemaining <= 0;
		[JsonIgnore] public bool IsDestroyed => Health <= 0;
		[JsonIgnore] public float HealthPercentage => (float)Health / HealthMax;

		public string ShipType { get; set; } = "scout";
		public int HealthMax { get; set; } = 100;
		public float MovementSpeed { get; set; } = 8.0f;
		public float Radius { get; set; } = 10.0f;
		public float RotationSpeed { get; set; } = 10.0f;

		public bool HasBeams { get; set; } = true;
		public float BeamsRange { get; set; } = 300;
		public int BeamsCooldownWait { get; set; } = 10;
		public int BeamsMinDamage { get; set; } = 10;
		public int BeamsMaxDamage { get; set; } = 10;
		public int BeamsCooldownRemaining { get; set; } = 10;

		public ShipAction Action { get; set; }
		public Vector2 ActionPosition { get; set; }
		public LocalId ActionTarget { get; set; }

		public bool IsInBeamsRange(GameplayShip other)
		{
			return (Vector2.Distance(Position, other.Position) - Radius - other.Radius) <= BeamsRange;
		}
	}
}
