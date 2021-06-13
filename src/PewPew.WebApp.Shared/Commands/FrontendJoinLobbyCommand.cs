namespace PewPew.WebApp.Shared.Commands
{
	public class FrontendJoinLobbyCommand : ClientCommand
	{
		public string LobbyKey { get; set; } = string.Empty;
	}
}
