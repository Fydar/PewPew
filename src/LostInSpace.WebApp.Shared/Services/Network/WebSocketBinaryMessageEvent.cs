using System;
using System.IO;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class WebSocketBinaryMessageEvent : IWebSocketEvent
	{
		public DateTimeOffset StartTime { get; internal set; }
		public DateTimeOffset EndTime { get; internal set; }
		public Stream Body { get; internal set; }

		public TimeSpan Elapsed => EndTime - StartTime;
	}
}
