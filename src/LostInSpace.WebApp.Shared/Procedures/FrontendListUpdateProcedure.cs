using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class FrontendListUpdateProcedure : NetworkedViewProcedure
	{
		public List<LobbyStatus> Lobbies { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view is ClientNetworkedView clientView)
			{
				clientView.Lobbies = Lobbies;
			}
		}
	}
}
