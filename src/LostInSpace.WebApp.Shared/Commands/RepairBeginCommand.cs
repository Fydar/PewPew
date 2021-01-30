using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class RepairBeginCommand : ClientCommand
	{
		public LocalId Target { get; set; }
	}
}
