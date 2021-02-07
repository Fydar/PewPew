using PewPew.WebApp.Shared.View;

namespace PewPew.WebApp.Shared.Commands
{
	public class UseAbilityCommand : ClientCommand
	{
		public Vector2 TargetLocation { get; set; }
	}
}
