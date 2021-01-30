using Husky.Game.Shared.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// The scene of gameplay-related elements.
	/// </summary>
	public class GameplayWorld
	{
		public Dictionary<LocalId, GameplayShip> Ships { get; set; } = new Dictionary<LocalId, GameplayShip>();
	}
}
