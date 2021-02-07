using PewPew.WebApp.Shared.Model;
using Newtonsoft.Json;

namespace PewPew.WebApp.Shared.View
{
	/// <summary>
	/// Represents a playable character controlled by a player.
	/// </summary>
	public class GameplayShip
	{
		public Vector2 Position { get; set; }
		public float Rotation { get; set; }
		public int Health { get; set; } = 100;

		[JsonIgnore] public bool CanShoot => HasBeams && BeamsCooldownRemaining <= 0;
		[JsonIgnore] public bool IsDestroyed => Health <= 0;
		[JsonIgnore] public float HealthPercentage => (float)Health / HealthMax;
		[JsonIgnore] public bool CanBarrage => !IsDestroyed && HasBarrage && BarrageCooldownRemaining <= 0;

		public string ShipType { get; set; } = "scout";
		public int HealthMax { get; set; } = 100;
		public float MovementSpeed { get; set; } = 8.0f;
		public float Radius { get; set; } = 10.0f;
		public float RotationSpeed { get; set; } = 10.0f;

		public bool HasBeams { get; set; } = true;
		public float BeamsRange { get; set; } = 300;
		public int BeamsCooldownWait { get; set; } = 8;
		public int BeamDamagePerTick { get; set; } = 5;
		public int BeamThickness { get; set; } = 4;
		public int BeamsCooldownRemaining { get; set; } = 10;

		public bool HasBarrage { get; set; } = false;
		public int BarrageCoodownWait { get; set; } = 30;
		public int BarrageCooldownRemaining { get; set; } = 30;
		public int BarrageProjectiles { get; set; } = 8;
		public int BarrageDamagePerProjectile { get; set; } = 35;
		public float BarrageRadius { get; set; } = 150.0f;

		public ShipAction Action { get; set; }
		public Vector2 ActionPosition { get; set; }
		public LocalId ActionTarget { get; set; }

		public bool IsInBeamsRange(GameplayShip other)
		{
			return (Vector2.Distance(Position, other.Position) - Radius - other.Radius) <= BeamsRange;
		}
	}
}
