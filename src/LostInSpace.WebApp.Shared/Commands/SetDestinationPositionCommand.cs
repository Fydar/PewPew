using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class SetDestinationPositionCommand : ClientCommand
	{
		public Vector2 Position { get; set; }
	}
}
