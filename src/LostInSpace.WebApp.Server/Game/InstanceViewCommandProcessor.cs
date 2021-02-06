using LostInSpace.WebApp.Server.Services;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Model;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LostInSpace.WebApp.Server.Game
{
	public class InstanceViewCommandProcessor
	{
		private readonly ServerNetworkedView view;
		private readonly Random random;

		public InstanceViewCommandProcessor(ServerNetworkedView view)
		{
			this.view = view;

			random = new Random();
		}

		public IEnumerable<ScopedNetworkedViewProcedure> HandleGameTick()
		{
			if (view.Lobby.World?.Ships == null)
			{
				yield break;
			}

			var world = view.Lobby.World;

			int teamAAlive = view.Lobby.World.Ships.Where(s => !s.Value.IsDestroyed).Count(p => view.Lobby.Players[p.Key].TeamId == 0);
			int teamBAlive = view.Lobby.World.Ships.Where(s => !s.Value.IsDestroyed).Count(p => view.Lobby.Players[p.Key].TeamId == 1);

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

			foreach (var projectileKvp in world.Projectiles)
			{
				var projectile = projectileKvp.Value;

				if (projectile.IsDestroyed)
				{
					continue;
				}

				/*if (projectile.LifetimeRemaining <= 0)
				{
					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new ProjectileDestroyProcedure()
						{
							ProjectileId = projectileKvp.Key
						}
					);
				}*/

				foreach (var shipKvp in world.Ships)
				{
					if (shipKvp.Key == projectile.Owner
						|| shipKvp.Value.IsDestroyed)
					{
						continue;
					}

					// Ignore teammates
					if (view.Lobby.Players.TryGetValue(projectile.Owner, out var thisOwnership))
					{
						if (view.Lobby.Players.TryGetValue(shipKvp.Key, out var thisPlayer))
						{
							if (thisPlayer.TeamId == thisOwnership.TeamId)
							{
								continue;
							}
						}
					}

					var ship = shipKvp.Value;

					if (Vector2.Distance(projectile.Position, ship.Position) - ship.Radius <= 2.0f)
					{
						yield return new ScopedNetworkedViewProcedure(
							ProcedureScope.Broadcast,
							new ProjectileDestroyProcedure()
							{
								ProjectileId = projectileKvp.Key
							}
						);

						yield return new ScopedNetworkedViewProcedure(
							ProcedureScope.Broadcast,
							new ShipDamageProcedure()
							{
								Damage = projectile.Damage,
								Source = projectile.Owner,
								Target = shipKvp.Key
							}
						);
						break;
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

					float targetAngle;
					if (direction.Magnitude > 4.0f)
					{
						targetAngle = Vector2.SignedAngle(Vector2.up, direction.Normalized);
					}
					else
					{
						targetAngle = ship.Rotation;
					}

					float currentAngle = Mathf.MoveTowardsAngle(ship.Rotation, targetAngle, ship.RotationSpeed);

					var newPosition = Vector2.MoveTowards(ship.Position, targetPosition, ship.MovementSpeed);

					yield return new ScopedNetworkedViewProcedure(
						ProcedureScope.Broadcast,
						new UpdatePositionProcedure()
						{
							Identifier = shipKvp.Key,
							Position = newPosition,
							Rotation = currentAngle
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

						// Ignore teammates
						if (view.Lobby.Players.TryGetValue(otherShipKvp.Key, out var otherPlayer))
						{
							if (view.Lobby.Players.TryGetValue(shipKvp.Key, out var thisPlayer))
							{
								if (thisPlayer.TeamId == otherPlayer.TeamId)
								{
									continue;
								}
							}
						}

						if (ship.IsInBeamsRange(otherShip))
						{
							var offsetDirection = new Vector2(
								((float)random.NextDouble() - 0.5f) * 2.0f,
								((float)random.NextDouble() - 0.5f) * 2.0f).Normalized;

							float offsetLength = (float)random.NextDouble();

							var hitPoint = otherShip.Position
								+ (offsetDirection * offsetLength * otherShip.Radius * 1.0f);

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
										DamagePerTick = ship.BeamDamagePerTick,
										Thickness = ship.BeamThickness
									}
								}
							);

							break;
						}
					}
				}
			}

			yield return new ScopedNetworkedViewProcedure(
				ProcedureScope.Broadcast,
				new GameTickProcedure()
				{
				}
			);
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
				new LobbyReplicateProcedure()
				{
					Lobby = view.Lobby
				}
			);

			int playerTeam;

			int teamACount = view.Lobby.Players.Count(p => p.Value.TeamId == 0);
			int teamBCount = view.Lobby.Players.Count(p => p.Value.TeamId == 1);

			if (teamACount <= teamBCount)
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
						DisplayName = RandomName.Generate(),
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
				case LaunchGameCommand:
				{
					var world = new GameplayWorld();

					foreach (var player in view.Lobby.Players)
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
							ship.Radius = 70;
							ship.MovementSpeed = 5.0f;

							ship.HealthMax = 700;
							ship.Health = 700;

							ship.BeamsRange = 550;
							ship.BeamDamagePerTick = 15;
							ship.BeamThickness = 5;

							ship.HasBarrage = true;
							ship.BarrageRadius = 150.0f;
							ship.BarrageProjectiles = 6;
							ship.BarrageCooldownRemaining = 0;
							ship.BarrageCoodownWait = 60;
						}
						else if (ship.ShipType == ShipTypes.Scout)
						{
							ship.Radius = 36;
							ship.MovementSpeed = 14.0f;

							ship.HealthMax = 100;
							ship.Health = 100;

							ship.BeamDamagePerTick = 10;
							ship.BeamThickness = 2;
							ship.BeamsRange = 500;
							ship.BeamsCooldownWait = 6;

							ship.HasBarrage = false;
						}
						else if (ship.ShipType == ShipTypes.Gunship)
						{
							ship.Radius = 40;
							ship.MovementSpeed = 7.0f;

							ship.HealthMax = 100;
							ship.Health = 100;

							ship.BeamDamagePerTick = 16;
							ship.BeamThickness = 3;
							ship.BeamsRange = 600;

							ship.HasBarrage = true;
							ship.BarrageRadius = 50.0f;
							ship.BarrageProjectiles = 1;
							ship.BarrageCooldownRemaining = 0;
							ship.BarrageCoodownWait = 60;
						}
						else if (ship.ShipType == ShipTypes.Battleship)
						{
							ship.Radius = 40;
							ship.MovementSpeed = 5.5f;

							ship.HealthMax = 300;
							ship.Health = 300;

							ship.BeamDamagePerTick = 10;
							ship.BeamThickness = 2;
							ship.BeamsRange = 400;

							ship.HasBarrage = false;

							ship.HasBarrage = true;
							ship.BarrageRadius = 50.0f;
							ship.BarrageProjectiles = 3;
							ship.BarrageCooldownRemaining = 0;
							ship.BarrageCoodownWait = 40;
							ship.BarrageDamagePerProjectile = 10;
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

				case UseAbilityCommand command:
				{
					var ship = view.Lobby.World.Ships[connection.Identifier];

					for (int i = 0; i < ship.BarrageProjectiles; i++)
					{
						var offsetDirection = new Vector2(
							((float)random.NextDouble() - 0.5f) * 2.0f,
							((float)random.NextDouble() - 0.5f) * 2.0f).Normalized;

						float offsetLength = (float)random.NextDouble();

						var hitPoint = command.TargetLocation
							+ (offsetDirection * offsetLength * ship.BarrageRadius);

						var shootDirection = (hitPoint - ship.Position).Normalized;

						float shootAngle = Vector2.SignedAngle(Vector2.up, shootDirection.Normalized);

						yield return new ScopedNetworkedViewProcedure(
							ProcedureScope.Broadcast,
							new ProjectileFireProcedure()
							{
								ProjectileId = LocalId.NewId(),
								Projectile = new Projectile()
								{
									LifetimeRemaining = 80,
									Owner = connection.Identifier,
									Position = ship.Position,
									Rotation = shootAngle,
									Velocity = shootDirection * 72.0f,
									Damage = ship.BarrageDamagePerProjectile
								}
							}
						);
					}

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
			}
		}
	}
}
