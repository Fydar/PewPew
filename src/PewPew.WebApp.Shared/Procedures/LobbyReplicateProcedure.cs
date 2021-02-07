using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyReplicateProcedure : NetworkedViewProcedure
	{
		public LobbyView Lobby { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view is ClientNetworkedView)
			{
				view.Lobby = Lobby;
			}
		}
	}
}
