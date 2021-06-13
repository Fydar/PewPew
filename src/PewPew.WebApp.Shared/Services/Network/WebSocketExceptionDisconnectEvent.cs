using System;

namespace PewPew.WebApp.Shared.Services.Network
{
	public class WebSocketExceptionDisconnectEvent : WebSocketDisconnectEvent
	{
		public Exception InnerException { get; }

		public WebSocketExceptionDisconnectEvent(Exception innerException)
		{
			InnerException = innerException;
		}
	}
}
