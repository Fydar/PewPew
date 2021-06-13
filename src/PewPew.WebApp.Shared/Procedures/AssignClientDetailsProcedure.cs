using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class AssignClientDetailsProcedure : NetworkedViewProcedure
	{
		public ClientSpecific? ClientDetails { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (ClientDetails == null)
			{
				throw new InvalidOperationException("Cannot apply procedure as it is malformed.");
			}

			if (view is ClientNetworkedView clientNetworkedView)
			{
				clientNetworkedView.Client = ClientDetails;
			}
		}
	}
}
