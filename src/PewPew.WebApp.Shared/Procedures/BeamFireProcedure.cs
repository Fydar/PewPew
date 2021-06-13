using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class BeamFireProcedure : NetworkedViewProcedure
	{
		public LocalId BeamId { get; set; }
		public Beam? Beam { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (Beam == null)
			{
				throw new InvalidOperationException("Cannot apply procedure as it is malformed.");
			}

			if (view.Lobby?.World == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid world.");
			}

			var owner = view.Lobby.World.Ships[Beam.Owner];
			owner.BeamsCooldownRemaining = owner.BeamsCooldownWait;

			view.Lobby.World.Beams.Add(BeamId, Beam);
		}
	}
}
