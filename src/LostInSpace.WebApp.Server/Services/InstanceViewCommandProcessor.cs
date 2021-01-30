using Husky.Game.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Server.Services
{
	public enum ProcedureScope
	{
		Broadcast,
		Reply,
		Forward
	}

	public struct ScopedNetworkedViewProcedure
	{
		public ProcedureScope Scope { get; set; }
		public NetworkedViewProcedure Procedure { get; set; }

		public ScopedNetworkedViewProcedure(ProcedureScope scope, NetworkedViewProcedure procedure)
		{
			Scope = scope;
			Procedure = procedure;
		}
	}

	public class InstanceViewCommandProcessor
	{
		private readonly NetworkedView networkedView;

		public InstanceViewCommandProcessor(NetworkedView clientView)
		{
			networkedView = clientView;
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerConnect(GameClientConnection connection)
		{
			yield break;
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Reply,
				null
			);
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerDisconnect(GameClientConnection connection)
		{
			yield break;
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				null
			);
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandleRecieveCommand(GameClientConnection connection, ClientCommand command)
		{
			yield break;
		}
	}
}
