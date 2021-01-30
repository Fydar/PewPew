using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Procedures
{
	public class LobbyPlayerJoinProcedure : NetworkedViewProcedure
	{
		public LocalId Identifier { get; set; }
		public string DisplayName { get; set; }

		public override void ApplyToView(NetworkedView view)
		{
			var player = new LobbyPublicPlayerProfile()
			{
				Identifier = Identifier,
				DisplayName = DisplayName
			};

			view.Lobby.Players.Add(player.Identifier, player);

			view.Lobby.Chat.OnReceiveMessage?.Invoke(new ChatMessage("System", $"\"{DisplayName}\" joined the lobby."));
		}
	}
}
