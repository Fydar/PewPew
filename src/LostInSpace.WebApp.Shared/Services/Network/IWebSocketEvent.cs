using System;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public interface IWebSocketEvent : IDisposable
	{
		DateTimeOffset StartTime { get; }
		DateTimeOffset EndTime { get; }
		TimeSpan Elapsed { get; }
	}
}
