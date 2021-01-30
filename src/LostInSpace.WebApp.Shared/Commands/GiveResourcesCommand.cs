using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class GiveResourcesCommand : ClientCommand
	{
		public LocalId GiveTo { get; set; }

		public int FuelToGive { get; set; }
		public int SupplyToGive { get; set; }
	}
}
