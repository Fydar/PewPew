using System;
using System.Buffers;

namespace PewPew.WebApp.Shared.Services.Network
{
	public class WebSocketBinaryMessageEvent : IWebSocketEvent
	{
		private readonly byte[] rentedArray;

		public DateTimeOffset StartTime { get; internal set; }
		public DateTimeOffset EndTime { get; internal set; }
		public ArraySegment<byte> Body { get; internal set; }

		public TimeSpan Elapsed => EndTime - StartTime;

		internal WebSocketBinaryMessageEvent(byte[] rentedArray)
		{
			this.rentedArray = rentedArray;
		}

		void IDisposable.Dispose()
		{
			ArrayPool<byte>.Shared.Return(rentedArray);
			GC.SuppressFinalize(this);
		}
	}
}
