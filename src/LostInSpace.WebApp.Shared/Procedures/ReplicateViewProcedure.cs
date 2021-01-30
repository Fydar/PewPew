using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class ReplicateViewProcedure : NetworkedViewProcedure
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
