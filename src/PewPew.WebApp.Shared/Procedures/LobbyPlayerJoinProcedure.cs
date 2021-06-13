using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.View;
using System;

namespace PewPew.WebApp.Shared.Procedures
{
	public class LobbyPlayerJoinProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public LobbyPublicPlayerProfile? Profile { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			if (Profile == null)
			{
				throw new InvalidOperationException("Cannot apply procedure as it is malformed.");
			}

			if (view.Lobby == null)
			{
				throw new InvalidOperationException("Cannot apply procedure to networked view as it doesn't have a valid lobby.");
			}

			view.Lobby.Players.Add(Identifier, Profile);

			view.Lobby.Chat.OnReceiveMessage?.Invoke(new ChatMessage("System", $"\"{Profile.DisplayName}\" joined the lobby."));
		}
	}
}
