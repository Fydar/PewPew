using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class LobbyPlayerJoinProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public LobbyPublicPlayerProfile Profile { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			view.Lobby.Players.Add(Identifier, Profile);

			view.Lobby.Chat.OnReceiveMessage?.Invoke(new ChatMessage("System", $"\"{Profile.DisplayName}\" joined the lobby."));
		}
	}
}
