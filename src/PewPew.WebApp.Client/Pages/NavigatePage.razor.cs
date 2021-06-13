using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.Procedures;
using PewPew.WebApp.Shared.View;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class NavigatePage : ComponentBase, IDisposable
	{
		public enum NavigateInputMode
		{
			None,
			SetDestination,
			Ability,
		}

		[Inject] protected ClientService? Client { get; set; }
		[Inject] protected NavigationManager? NavigationManager { get; set; }
		[Inject] protected IJSRuntime? JsRuntime { get; set; }

		public NavigateInputMode InputMode
		{
			get
			{
				EnsureValidState();

				if (Client.View?.Client == null)
				{
					return NavigateInputMode.None;
				}

				if (Client.View?.Lobby?.World?.Ships.ContainsKey(Client.View.Client.ClientId) ?? false)
				{
					if (UseAbility)
					{
						return NavigateInputMode.Ability;
					}
					return NavigateInputMode.SetDestination;
				}
				else
				{
					return NavigateInputMode.None;
				}
			}
		}

		public bool UseAbility { get; set; } = false;

		public int YourTeam
		{
			get
			{
				EnsureValidState();

				if (Client.View?.Client == null)
				{
					return -1;
				}

				return GetTeamId(Client.View.Client.ClientId);
			}
		}

		public int GetTeamId(LocalId localId)
		{
			EnsureValidState();

			return Client.View?.Lobby?.Players[localId].TeamId ?? -1;
		}

		public LocalId? CurrentlySelected { get; set; }

		public GameplayShip? YourShip
		{
			get
			{
				EnsureValidState();

				if (Client.View?.Client == null
					|| Client.View?.Lobby?.World == null)
				{
					return null;
				}

				Client.View.Lobby.World.Ships.TryGetValue(Client.View.Client.ClientId, out var result);
				return result;
			}
		}

		protected override Task OnInitializedAsync()
		{
			EnsureValidState();

			Client.OnProcedureApplied += OnProcedureApplied;

			if (Client.View?.Lobby == null)
			{
				NavigationManager.NavigateTo("/");
			}
			else if (Client.View?.Lobby?.World == null)
			{
				NavigationManager.NavigateTo("/lobby");
			}

			return Task.CompletedTask;
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			EnsureValidState();

			if (firstRender)
			{
				await JsRuntime.InvokeAsync<object>("PanAndZoom.Start");
			}
		}

		public void AbilityButtonClicked(MouseEventArgs mouseEventArgs)
		{
			if (YourShip != null)
			{
				if (YourShip.CanBarrage)
				{
					UseAbility = true;
				}
			}
		}

		public void MapOnClick(MouseEventArgs mouseEventArgs)
		{
			EnsureValidState();
		}

		public void MapOnMouseUp(MouseEventArgs mouseEventArgs)
		{
			EnsureValidState();

			if (InputMode == NavigateInputMode.SetDestination)
			{
				if (mouseEventArgs.Button == 2)
				{
					_ = Client.SendCommandAsync(new SetDestinationPositionCommand()
					{
						Position = new Vector2((float)mouseEventArgs.OffsetX, (float)mouseEventArgs.OffsetY)
					});
				}
			}
			else if (InputMode == NavigateInputMode.Ability)
			{
				if (mouseEventArgs.Button != 1)
				{
					_ = Client.SendCommandAsync(new UseAbilityCommand()
					{
						TargetLocation = new Vector2((float)mouseEventArgs.OffsetX, (float)mouseEventArgs.OffsetY)
					});

					UseAbility = false;
				}
			}
		}

		public void MapOnTouchEnd(TouchEventArgs touchEventArgs)
		{
			EnsureValidState();
		}

		public void Dispose()
		{
			EnsureValidState();

			Client.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure? networkedViewProcedure)
		{
			EnsureValidState();

			if (networkedViewProcedure is GameTickProcedure)
			{
				_ = InvokeAsync(StateHasChanged);
			}

			if (Client.View?.Lobby?.World == null)
			{
				NavigationManager.NavigateTo("/lobby");
			}
		}

		[MemberNotNull(nameof(Client), nameof(NavigationManager), nameof(JsRuntime))]
		private void EnsureValidState([CallerMemberName] string callerMemberName = "")
		{
			if (Client == null)
			{
				throw CreateMissingPropertyException(nameof(Client));
			}
			if (NavigationManager == null)
			{
				throw CreateMissingPropertyException(nameof(NavigationManager));
			}
			if (JsRuntime == null)
			{
				throw CreateMissingPropertyException(nameof(JsRuntime));
			}

			Exception CreateMissingPropertyException(string propertyName)
			{
				return new InvalidOperationException($"Cannot execute '{callerMemberName}' on {nameof(LobbyPage)}' as the property {propertyName} has not been injected.");
			}
		}
	}
}
