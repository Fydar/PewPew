using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class GameStartProcedure : NetworkedViewProcedure
	{
		public GameplayWorld? World { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid lobby.");
			}

			view.Lobby.World = World;
		}
	}
}
