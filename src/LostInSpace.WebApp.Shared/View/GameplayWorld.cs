using Husky.Game.Shared.Model;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// The scene of gameplay-related elements.
	/// </summary>
	public class GameplayWorld
	{
		public Dictionary<LocalId, GameplayShip> Ships { get; set; } = new Dictionary<LocalId, GameplayShip>();
		public Dictionary<LocalId, Projectile> Projectiles { get; set; } = new Dictionary<LocalId, Projectile>();
		public Dictionary<LocalId, Beam> Beams { get; set; } = new Dictionary<LocalId, Beam>();

		public List<string> BattleLog { get; set; } = new List<string>();
	}
}
