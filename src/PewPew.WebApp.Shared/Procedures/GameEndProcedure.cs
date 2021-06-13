using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class GameEndProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby != null)
			{
				view.Lobby.World = null;
			}
		}
	}
}
