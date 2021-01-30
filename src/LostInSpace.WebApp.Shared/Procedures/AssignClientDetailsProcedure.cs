using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class AssignClientDetailsProcedure : NetworkedViewProcedure
	{
		public ClientSpecific ClientDetails { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view is ClientNetworkedView clientNetworkedView)
			{
				clientNetworkedView.Client = ClientDetails;
			}
		}
	}
}
