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
		public int CooldownRemaining { get; set; } = 10;

		[JsonIgnore] public bool CanShoot => CooldownRemaining <= 0;
		[JsonIgnore] public bool IsDestroyed => Health <= 0;
		[JsonIgnore] public float HealthPercentage => (float)Health / HealthMax;

		public int FuelUnits { get; set; }
		public int SupplyUnits { get; set; }

		public int HealthMax { get; set; } = 100;
		public int CooldownWait { get; set; } = 10;
		public int MinDamage { get; set; } = 10;
		public int MaxDamage { get; set; } = 10;
		public float MovementSpeed { get; set; } = 8.0f;
		public float Radius { get; set; } = 10.0f;
		public string Style { get; set; } = "ship-scout";

		public ShipAction Action { get; set; }
		public Vector2 ActionPosition { get; set; }
		public LocalId ActionTarget { get; set; }

		public bool IsInDockRange(GameplayShip other)
		{
			const float dockRange = 16.0f;

			return Vector2.Distance(Position, other.Position) <= dockRange;
		}

		public bool IsInCommsRange(GameplayShip other)
		{
			const float commsRange = 256.0f;

			return Vector2.Distance(Position, other.Position) <= commsRange;
		}
	}
}
