using System;
using System.Threading;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public interface INetworkChannel
	{
		event Action OnConnected;

		event Action<NetworkChannelMessage> OnReceive;

		bool IsConnected { get; }

		Task CloseAsync(CancellationToken cancellationToken = default);

		Task SendAsync(byte[] message, CancellationToken cancellationToken = default);
	}
}
