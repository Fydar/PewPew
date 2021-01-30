using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class RepairBeginProcedure : NetworkedViewProcedure
	{
		public LocalId RepairFrom { get; set; }
		public LocalId RepairTo { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
		}
	}
}
