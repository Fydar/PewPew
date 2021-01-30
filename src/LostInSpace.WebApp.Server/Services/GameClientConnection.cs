using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.Services.Network;

namespace LostInSpace.WebApp.Server.Services
{
	public class GameClientConnection
	{
		public LocalId Identifier { get; }
		public INetworkChannel Connection { get; }

		public GameClientConnection(LocalId identifier, INetworkChannel connection)
		{
			Identifier = identifier;
			Connection = connection;
		}
	}
}
