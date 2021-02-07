using PewPew.WebApp.Shared.Procedures;

namespace PewPew.WebApp.Server.Game
{
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
}
