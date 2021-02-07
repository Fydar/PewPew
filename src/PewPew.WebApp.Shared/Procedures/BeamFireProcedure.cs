using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class BeamFireProcedure : NetworkedViewProcedure
	{
		public LocalId BeamId { get; set; }
		public Beam Beam { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var owner = view.Lobby.World.Ships[Beam.Owner];
			owner.BeamsCooldownRemaining = owner.BeamsCooldownWait;

			view.Lobby.World.Beams.Add(BeamId, Beam);
		}
	}
}
