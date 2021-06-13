using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyReplicateProcedure : NetworkedViewProcedure
	{
		public LobbyView? Lobby { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure as it is malformed.");
			}

			if (view is ClientNetworkedView)
			{
				view.Lobby = Lobby;
			}
		}
	}
}
