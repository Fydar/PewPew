using System;
using System.Buffers;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class WebSocketBinaryMessageEvent : IWebSocketEvent
	{
		internal byte[] rentedArray;

		public DateTimeOffset StartTime { get; internal set; }
		public DateTimeOffset EndTime { get; internal set; }
		public ArraySegment<byte> Body { get; internal set; }

		public TimeSpan Elapsed => EndTime - StartTime;

		void IDisposable.Dispose()
		{
			ArrayPool<byte>.Shared.Return(rentedArray);
		}
	}
}
