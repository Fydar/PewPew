using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using System;
using System.Collections.Generic;

namespace LostInSpace.WebApp.Server.Services
{
	public class InstanceViewCommandProcessor
	{
		private readonly NetworkedView networkedView;
		private readonly Random random;

		public InstanceViewCommandProcessor(NetworkedView clientView)
		{
			networkedView = clientView;

			random = new Random();
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandleGameTick()
		{
			if (networkedView.Lobby.World?.Ships == null)
			{
				yield break;
			}

			var world = networkedView.Lobby.World;

			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new GameTickProcedure()
				{
				}
			);

			foreach (var projectileKvp in world.Projectiles)
			{
				var projectile = projectileKvp.Value;

				if (projectile.LifetimeRemaining <= 0)
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new ProjectileDestroyProcedure()
						{
							ProjectileId = projectileKvp.Key
						}
					);
				}

				foreach (var shipKvp in world.Ships)
				{
					if (shipKvp.Key == projectile.Owner)
					{
						continue;
					}

					var ship = shipKvp.Value;

					if (Vector2.Distance(projectile.Position, ship.Position) <= 16.0f)
					{
						yield return new ScopedNetworkedViewProcedure(
							ProcedureScope.Broadcast,
							new ProjectileDestroyProcedure()
							{
								ProjectileId = projectileKvp.Key
							}
						);
					}
				}
			}

			foreach (var beamKvp in world.Beams)
			{
				var beam = beamKvp.Value;

				if (beam.LifetimeRemaining >= 0)
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new ShipDamageProcedure()
						{
							Source = beam.Owner,
							Target = beam.Target,
							Damage = beam.DamagePerTick
						}
					);
				}
			}

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

					float targetAngle = Vector2.SignedAngle(Vector2.up, direction.Normalized);

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

				if (ship.CanShoot)
				{
					foreach (var otherShipKvp in world.Ships)
					{
						// Ignore self
						if (otherShipKvp.Key == shipKvp.Key)
						{
							continue;
						}

						var otherShip = otherShipKvp.Value;

						if (ship.IsInCommsRange(otherShip))
						{
							var offsetDirection = new Vector2(
								((float)random.NextDouble() - 0.5f) * 2.0f,
								((float)random.NextDouble() - 0.5f) * 2.0f).Normalized;

							float offsetLength = (float)random.NextDouble();

							var hitPoint = otherShip.Position
								+ (offsetDirection * offsetLength * otherShip.Radius * 2.0f);

							yield return new ScopedNetworkedViewProcedure(
								ProcedureScope.Broadcast,
								new BeamFireProcedure()
								{
									BeamId = LocalId.NewId(),
									Beam = new Beam()
									{
										LifetimeRemaining = 2,
										Owner = shipKvp.Key,
										Target = otherShipKvp.Key,
										StartPosition = ship.Position,
										EndPosition = hitPoint,
										DamagePerTick = 5,
									}
								}
							);

							/*
							var shootDirection = (otherShip.Position - ship.Position).Normalized;

							float shootAngle = Vector2.SignedAngle(Vector2.up, shootDirection.Normalized);

							yield return new ScopedNetworkedViewProcedure(
								ProcedureScope.Broadcast,
								new ProjectileFireProcedure()
								{
									ProjectileId = LocalId.NewId(),
									Projectile = new Projectile()
									{
										LifetimeRemaining = 10,
										Owner = shipKvp.Key,
										Position = ship.Position,
										Rotation = shootAngle,
										Velocity = shootDirection * 24.0f
									}
								}
							);*/
							break;
						}
					}
				}
			}
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerConnect(GameClientConnection connection)
		{
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Reply,
				new AssignClientDetailsProcedure()
				{
					ClientDetails = new ClientSpecific()
					{
						ClientId = connection.Identifier
					}
				}
			);
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
				case LaunchGameCommand command:
				{
					var world = new GameplayWorld();

					foreach (var player in networkedView.Lobby.Players)
					{
						var ship = new GameplayShip
						{
							Position = new Vector2(
							random.Next(0, 512),
							random.Next(0, 512)
						),

							SupplyUnits = random.Next(256, 512),
							FuelUnits = random.Next(256, 512)
						};

						world.Ships.Add(player.Key, ship);
					}

					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new GameStartProcedure()
						{
							World = world
						}
					);
					break;
				}

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
