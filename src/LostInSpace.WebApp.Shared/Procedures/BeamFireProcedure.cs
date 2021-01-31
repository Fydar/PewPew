using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class BeamFireProcedure : NetworkedViewProcedure
	{
		public LocalId BeamId { get; set; }
		public Beam Beam { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var owner = view.Lobby.World.Ships[Beam.Owner];
			owner.CooldownRemaining = owner.CooldownWait;

			view.Lobby.World.Beams.Add(BeamId, Beam);
		}
	}
}
