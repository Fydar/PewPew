using System;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public interface IWebSocketEvent
	{
		DateTimeOffset StartTime { get; }
		DateTimeOffset EndTime { get; }
		TimeSpan Elapsed { get; }
	}
}
