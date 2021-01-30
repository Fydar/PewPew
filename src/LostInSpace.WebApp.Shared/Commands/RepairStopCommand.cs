using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class RepairStopCommand : ClientCommand
	{
		public LocalId Target { get; set; }
	}
}
