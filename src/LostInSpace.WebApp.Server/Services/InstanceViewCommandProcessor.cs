using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Server.Services
{
	public class InstanceViewCommandProcessor
	{
		private readonly NetworkedView networkedView;

		public InstanceViewCommandProcessor(NetworkedView clientView)
		{
			networkedView = clientView;
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandleGameTick()
		{
			if (networkedView.Lobby.World?.Ships == null)
			{
				yield break;
			}

			var world = networkedView.Lobby.World;

			foreach (var shipKvp in world.Ships)
			{
				var ship = shipKvp.Value;

				bool shouldMove = false;
				var targetPosition = ship.Position;

				switch (ship.Action)
				{
					case ShipAction.MoveToPosition:
					{
						shouldMove = true;
						targetPosition = ship.ActionPosition;
						break;
					}

					case ShipAction.MoveToShip:
					case ShipAction.DockToShip:
					case ShipAction.RepairDockedShip:
					{
						var targetShip = world.Ships[ship.ActionTarget];

						shouldMove = true;
						targetPosition = targetShip.Position;
						break;
					}
				}

				if (shouldMove)
				{
					var direction = targetPosition - ship.Position;
					float targetAngle = Vector2.Angle(Vector2.up, direction.Normalized);

					var newPosition = Vector2.MoveTowards(ship.Position, targetPosition, ship.MovementSpeed);

					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new UpdatePositionProcedure()
						{
							Identifier = shipKvp.Key,
							Position = newPosition,
							Rotation = targetAngle
						}
					);
				}
			}
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerConnect(GameClientConnection connection)
		{
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Reply,
				new ReplicateViewProcedure()
				{
					Lobby = networkedView.Lobby
				}
			);
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new LobbyPlayerJoinProcedure()
				{
					Identifier = connection.Identifier,
					DisplayName = connection.Identifier.ToString()
				}
			);
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerDisconnect(GameClientConnection connection)
		{
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new LobbyPlayerLeaveProcedure()
				{
					Identifier = connection.Identifier
				}
			);
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandleRecieveCommand(GameClientConnection connection, ClientCommand clientCommand)
		{
			switch (clientCommand)
			{
				case UpdateDisplayNameCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new LobbyPlayerUpdateNameProcedure()
						{
							Identifier = connection.Identifier,
							DisplayName = command.DisplayName
						}
					);
					break;
				}

				case SetDestinationPositionCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new SetDestinationPositionProcedure()
						{
							Identifier = connection.Identifier,
							Position = command.Position
						}
					);
					break;
				}

				case SetDestinationShipCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new SetDestinationShipProcedure()
						{
							Identifier = connection.Identifier,
							Target = command.Target
						}
					);
					break;
				}

				case RepairBeginCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new RepairBeginProcedure()
						{
							RepairFrom = connection.Identifier,
							RepairTo = command.Target
						}
					);
					break;
				}

				case RepairStopCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new RepairEndProcedure()
						{
							RepairFrom = connection.Identifier,
							RepairTo = command.Target,
						}
					);
					break;
				}

				case GiveResourcesCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new RepairEndProcedure()
						{
							RepairFrom = connection.Identifier,
							RepairTo = command.GiveTo,
						}
					);
					break;
				}
			}
		}
	}
}
