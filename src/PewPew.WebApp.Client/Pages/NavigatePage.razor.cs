using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Model;
using PewPew.WebApp.Shared.Procedures;
using PewPew.WebApp.Shared.View;
using System;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class NavigatePage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService Client { get; set; }
		[Inject] protected NavigationManager NavigationManager { get; set; }
		[Inject] protected IJSRuntime JsRuntime { get; set; }

		public NavigateInputMode InputMode
		{
			get
			{
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
				return GetTeamId(Client.View.Client.ClientId);
			}
		}

		public int GetTeamId(LocalId localId)
		{
			return Client.View?.Lobby?.Players[localId].TeamId ?? -1;
		}

		public LocalId? CurrentlySelected { get; set; }

		public GameplayShip YourShip
		{
			get
			{
				if (Client.View?.Client == null
					|| Client.View?.Lobby?.World == null)
				{
					return null;
				}

				Client.View.Lobby.World.Ships.TryGetValue(Client.View.Client.ClientId, out var result);
				return result;
			}
		}

		public enum NavigateInputMode
		{
			None,
			SetDestination,
			Ability,
		}

		protected override Task OnInitializedAsync()
		{
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
		}

		public void MapOnMouseUp(MouseEventArgs mouseEventArgs)
		{
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
		}

		public void Dispose()
		{
			Client.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
			if (networkedViewProcedure is GameTickProcedure)
			{
				_ = InvokeAsync(StateHasChanged);
			}

			if (Client.View?.Lobby?.World == null)
			{
				NavigationManager.NavigateTo("/lobby");
			}
		}
	}
}
