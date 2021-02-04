using LostInSpace.WebApp.Shared.Model;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// A representation of a networked view throughout.
	/// </summary>
	public abstract class NetworkedView
	{
		/// <summary>
		/// The lobby that the player is currently connected.
		/// </summary>
		public LobbyView Lobby { get; set; }
	}

	/// <summary>
	/// A representation of a clients view throughout the lifetime of it's connection to the server.
	/// </summary>
	public sealed class ClientNetworkedView : NetworkedView
	{
		/// <summary>
		/// Client-specific functionality as-informed to this view by the server.
		/// </summary>
		public ClientSpecific Client { get; set; }

		public List<LobbyStatus> Lobbies { get; set; }
	}

	/// <summary>
	/// The servers view state.
	/// </summary>
	public sealed class ServerNetworkedView : NetworkedView
	{
	}
}
