using System;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class NetworkLogChannel
	{
		public event Action<NetworkLog> OnStart;
		public event Action<NetworkLog> OnComplete;

		public NetworkLog Start(string name)
		{
			var log = new NetworkLog(this, name).Start();

			OnStart?.Invoke(log);

			return log;
		}

		internal void InvokeOnComplete(NetworkLog log)
		{
			OnComplete?.Invoke(log);
		}
	}
}
