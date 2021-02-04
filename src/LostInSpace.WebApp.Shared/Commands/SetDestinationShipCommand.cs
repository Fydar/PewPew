using LostInSpace.WebApp.Shared.Model;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class SetDestinationShipCommand : ClientCommand
	{
		public LocalId Target { get; set; }
	}
}
