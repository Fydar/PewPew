using System;

namespace PewPew.WebApp.Shared.Services.Network
{
	public interface IWebSocketEvent : IDisposable
	{
		DateTimeOffset StartTime { get; }
		DateTimeOffset EndTime { get; }
		TimeSpan Elapsed { get; }
	}
}
