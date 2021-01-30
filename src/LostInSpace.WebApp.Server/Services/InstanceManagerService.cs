using LostInSpace.WebApp.Server.Services;

namespace HuskyNet.Instance.Server.Services
{
	public class InstanceManagerService
	{
		public GameServerWorker Worker { get; }

		public InstanceManagerService()
		{
			Worker = new GameServerWorker();
		}
	}
}
