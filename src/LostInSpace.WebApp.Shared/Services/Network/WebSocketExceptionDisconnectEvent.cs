using System;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class WebSocketExceptionDisconnectEvent : WebSocketDisconnectEvent
	{
		public Exception InnerException { get; internal set; }
	}
}
