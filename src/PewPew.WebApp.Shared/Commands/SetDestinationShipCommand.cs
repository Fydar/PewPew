using PewPew.WebApp.Shared.Model;

namespace PewPew.WebApp.Shared.Commands
{
	public class SetDestinationShipCommand : ClientCommand
	{
		public LocalId Target { get; set; }
	}
}
