using PewPew.WebApp.Shared.Model;

namespace PewPew.WebApp.Shared.Commands
{
	public class LobbyKickPlayerCommand : ClientCommand
	{
		public LocalId TargetId { get; set; }
	}
}
