using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public interface INetworkChannel
	{
		Task CloseAsync(CancellationToken cancellationToken = default);
		IAsyncEnumerable<IWebSocketEvent> ListenAsync(CancellationToken cancellationToken);
		Task SendAsync(byte[] message, CancellationToken cancellationToken = default);
	}
}
