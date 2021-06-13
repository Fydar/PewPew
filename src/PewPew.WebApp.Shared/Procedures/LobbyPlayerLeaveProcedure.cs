using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerLeaveProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (view.Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid lobby.");
			}

			var character = view.Lobby.Players[Identifier];

			view.Lobby.Players.Remove(Identifier);

			view.Lobby.Chat.OnReceiveMessage?.Invoke(new ChatMessage("System", $"\"{character.DisplayName}\" left the lobby."));
		}
	}
}
