using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.Services.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PewPew.WebApp.Server.Services
{
	public class GameClientConnection
	{
		public LocalId Identifier { get; }
		public ICommandProcessor? CommandProcessor { get; set; }

		private readonly INetworkChannel channel;

		public event Action<byte[]>? OnBeforeMessageSent;

		public GameClientConnection(LocalId identifier, INetworkChannel channel)
		{
			Identifier = identifier;
			this.channel = channel;
		}

		public Task SendAsync(byte[] message, CancellationToken cancellationToken = default)
		{
			OnBeforeMessageSent?.Invoke(message);

			return channel.SendAsync(message, cancellationToken);
		}

		public Task CloseAsync(CancellationToken cancellationToken = default)
		{
			return channel.CloseAsync(cancellationToken);
		}
	}
}
