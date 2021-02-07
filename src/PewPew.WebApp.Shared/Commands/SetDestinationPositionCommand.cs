using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Commands
{
	public class SetDestinationPositionCommand : ClientCommand
	{
		public Vector2 Position { get; set; }
	}
}
