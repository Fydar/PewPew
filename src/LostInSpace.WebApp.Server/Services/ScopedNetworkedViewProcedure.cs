using LostInSpace.WebApp.Shared.Procedures;

namespace LostInSpace.WebApp.Server.Services
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
