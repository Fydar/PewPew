using LostInSpace.WebApp.Shared.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace LostInSpace.WebApp.Shared.View
{
	/// <summary>
	/// Represents the information and events for an in-lobby chat.
	/// </summary>
	public class LobbyChat
	{
		[JsonIgnore]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public LobbyView Parent { get; set; }

		public Action<ChatMessage> OnReceiveMessage;
	}
}
