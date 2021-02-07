using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerLeaveProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var character = view.Lobby.Players[Identifier];

			view.Lobby.Players.Remove(Identifier);

			view.Lobby.Chat.OnReceiveMessage?.Invoke(new ChatMessage("System", $"\"{character.DisplayName}\" left the lobby."));
		}
	}
}
