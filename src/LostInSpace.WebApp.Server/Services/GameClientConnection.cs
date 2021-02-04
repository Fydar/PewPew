using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.Services.Network;

namespace LostInSpace.WebApp.Server.Services
{
	public class GameClientConnection
	{
		public LocalId Identifier { get; }
		public INetworkChannel Channel { get; }
		public ICommandProcessor CommandProcessor { get; set; }

		public GameClientConnection(LocalId identifier, INetworkChannel channel)
		{
			Identifier = identifier;
			Channel = channel;
		}
	}
}
