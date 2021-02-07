using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class GameEndProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby.World = null;
		}
	}
}
