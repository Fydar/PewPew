using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyCloseProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby = null;
		}
	}
}
