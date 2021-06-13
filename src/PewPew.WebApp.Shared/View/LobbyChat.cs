using PewPew.WebApp.Shared.Model;
using System;

namespace PewPew.WebApp.Shared.View
{
	/// <summary>
	/// Represents the information and events for an in-lobby chat.
	/// </summary>
	public class LobbyChat
	{
		public Action<ChatMessage>? OnReceiveMessage;
	}
}
