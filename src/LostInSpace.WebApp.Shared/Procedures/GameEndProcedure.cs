using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class GameEndProcedure : NetworkedViewProcedure
	{
		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby.World = null;
		}
	}
}
