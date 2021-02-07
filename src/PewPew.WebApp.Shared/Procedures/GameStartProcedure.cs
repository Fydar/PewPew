using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class GameStartProcedure : NetworkedViewProcedure
	{
		public GameplayWorld World { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby.World = World;
		}
	}
}
