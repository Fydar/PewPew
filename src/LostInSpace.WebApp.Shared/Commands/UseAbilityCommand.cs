using LostInSpace.WebApp.Shared.View;

namespace LostInSpace.WebApp.Shared.Commands
{
	public class UseAbilityCommand : ClientCommand
	{
		public Vector2 TargetLocation { get; set; }
	}
}
