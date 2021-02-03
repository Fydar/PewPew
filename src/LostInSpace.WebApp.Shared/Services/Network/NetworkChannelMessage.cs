using System.IO;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class NetworkChannelMessage
	{
		public INetworkChannel Channel { get; }
		public Stream Content { get; }

		public NetworkChannelMessage(INetworkChannel channel, Stream content)
		{
			Channel = channel;
			Content = content;
		}
	}
}
