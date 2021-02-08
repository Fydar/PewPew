using Newtonsoft.Json;
using PewPew.WebApp.Shared.Model;
using System;
using System.Diagnostics;

namespace PewPew.WebApp.Shared.View
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
