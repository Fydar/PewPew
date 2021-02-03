using Husky.Game.Shared.Model;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class LobbyKickPlayerCommand : ClientCommand
	{
		public LocalId TargetId { get; set; }
	}
}
