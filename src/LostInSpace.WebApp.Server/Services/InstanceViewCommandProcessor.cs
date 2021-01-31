﻿using Husky.Game.Shared.Model;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using System;
using System.Collections.Generic;
using System.Linq;

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

			int teamAAlive = networkedView.Lobby.World.Ships.Where(s => !s.Value.IsDestroyed).Count(p => networkedView.Lobby.Players[p.Key].TeamId == 0);
			int teamBAlive = networkedView.Lobby.World.Ships.Where(s => !s.Value.IsDestroyed).Count(p => networkedView.Lobby.Players[p.Key].TeamId == 1);

			if (teamAAlive == 0
				|| teamBAlive == 0)
			{
				yield return new ScopedNetworkedViewProcedure(
					ProcedureScope.Broadcast,
					new GameEndProcedure()
					{
					}
				);
				yield break;
			}

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
					if (shipKvp.Key == projectile.Owner
						|| shipKvp.Value.IsDestroyed)
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

				if (ship.IsDestroyed)
				{
					continue;
				}

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

						if (otherShip.IsDestroyed)
						{
							continue;
						}

						if (ship.IsInBeamsRange(otherShip))
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

			int playerTeam;

			int teamACount = networkedView.Lobby.Players.Count(p => p.Value.TeamId == 0);
			int teamBCount = networkedView.Lobby.Players.Count(p => p.Value.TeamId == 1);

			if (teamACount < teamBCount)
			{
				playerTeam = 0;
			}
			else
			{
				playerTeam = 1;
			}

			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new LobbyPlayerJoinProcedure()
				{
					Identifier = connection.Identifier,
					Profile = new LobbyPublicPlayerProfile()
					{
						DisplayName = RandomName.Get(),
						TeamId = playerTeam
					}
				}
			);
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandlePlayerDisconnect(GameClientConnection connection)
		{
			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new ShipDamageProcedure()
				{
					Source = connection.Identifier,
					Target = connection.Identifier,
					Damage = 10000
				}
			);

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
						Vector2 minBound, maxBound;

						if (player.Value.TeamId == 0)
						{
							minBound = new Vector2(512, 1920);
							maxBound = new Vector2(1024, 2176);
						}
						else
						{
							minBound = new Vector2(3072, 1920);
							maxBound = new Vector2(3584, 2176);
						}

						var ship = new GameplayShip
						{
							Position = new Vector2(
								random.Next((int)minBound.x, (int)maxBound.x),
								random.Next((int)minBound.y, (int)maxBound.y)
							),
							ShipType = player.Value.ShipClass,
						};

						if (ship.ShipType == ShipTypes.Capital)
						{
							ship.Radius = 32;
							ship.MovementSpeed = 3.0f;

							ship.BeamsRange = 400;
						}
						else if (ship.ShipType == ShipTypes.Scout)
						{

						}

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

				case LobbyUpdateTeamCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new LobbyPlayerUpdateTeamProcedure()
						{
							Identifier = connection.Identifier,
							TeamId = command.NewTeamId
						}
					);
					break;
				}

				case LobbyUpdateDisplayNameCommand command:
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

				case LobbyUpdateClassCommand command:
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new LobbyPlayerUpdateClassProcedure()
						{
							Identifier = connection.Identifier,
							ShipClass = command.ShipClass
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
