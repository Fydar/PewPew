using PewPew.WebApp.Shared.Commands;

namespace PewPew.WebApp.Server.Services
{
	public interface ICommandProcessor
	{
		void AddPlayer(GameClientConnection connection);
		void RecieveCommandFromPlayer(GameClientConnection connection, ClientCommand clientCommand);
		void RemovePlayer(GameClientConnection connection);
	}
}
