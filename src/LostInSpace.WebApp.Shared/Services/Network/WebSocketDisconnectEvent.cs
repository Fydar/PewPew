using System;
using System.Net.WebSockets;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class WebSocketDisconnectEvent : IWebSocketEvent
	{
		public DateTimeOffset StartTime { get; internal set; }
		public DateTimeOffset EndTime { get; internal set; }
		public WebSocketCloseStatus CloseStatus { get; set; }

		public TimeSpan Elapsed => EndTime - StartTime;

		void IDisposable.Dispose()
		{
		}
	}
}
