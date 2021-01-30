using Husky.Game.Shared.Model;
using System;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// Represents a playable character controlled by a player.
	/// </summary>
	public class GameplayCharacter
	{
		public LocalId Identifier { get; set; }

		public float PositionX { get; set; }
		public float PositionY { get; set; }
		public float PositionZ { get; set; }

		public float MovementInputX { get; set; }
		public float MovementInputY { get; set; }

		public float RotationX { get; set; }
		public float RotationY { get; set; }

		public bool IsCrouching { get; set; }

		public event Action OnPositionChanged;

		internal void InvokePositionChanged()
		{
			OnPositionChanged?.Invoke();
		}
	}
}
