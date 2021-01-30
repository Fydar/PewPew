using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	/// <summary>
	/// A remote-procedure to apply to a clients view.
	/// </summary>
	public abstract class NetworkedViewProcedure
	{
		public abstract void ApplyToView(NetworkedView view);
	}
}
