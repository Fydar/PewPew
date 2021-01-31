using Husky.Game.Shared.Model;
using HuskyNet.WebClient.Services;
using LostInSpace.WebApp.Shared.Commands;
using LostInSpace.WebApp.Shared.Procedures;
using LostInSpace.WebApp.Shared.View;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace LostInSpace.WebApp.Client.Pages
{
	public partial class NavigatePage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService ClientService { get; set; }
		[Inject] protected NavigationManager NavigationManager { get; set; }
		[Inject] protected IJSRuntime JsRuntime { get; set; }

		public NavigateInputMode InputMode
		{
			get
			{
				if (ClientService.View?.Client == null)
				{
					return NavigateInputMode.None;
				}

				if (ClientService.View?.Lobby?.World?.Ships.ContainsKey(ClientService.View.Client.ClientId) ?? false)
				{
					return NavigateInputMode.SetDestination;
				}
				else
				{
					return NavigateInputMode.None;
				}
			}
		}

		public int YourTeam
		{
			get
			{
				return GetTeamId(ClientService.View.Client.ClientId);
			}
		}

		public int GetTeamId(LocalId localId)
		{
			return ClientService.View?.Lobby?.Players[localId].TeamId ?? -1;
		}

		public LocalId? CurrentlySelected { get; set; }

		public enum NavigateInputMode
		{
			None,
			SetDestination,
		}

		protected override Task OnInitializedAsync()
		{
			ClientService.OnProcedureApplied += OnProcedureApplied;

			if (ClientService.View?.Lobby?.World == null)
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

		public void GotoButtonClick(MouseEventArgs mouseEventArgs)
		{
		}

		public void MapOnClick(MouseEventArgs mouseEventArgs)
		{
		}

		public void MapOnMouseUp(MouseEventArgs mouseEventArgs)
		{
			if (mouseEventArgs.Button != 2)
			{
				return;
			}

			if (InputMode == NavigateInputMode.SetDestination)
			{
				_ = ClientService.SendCommandAsync(new SetDestinationPositionCommand()
				{
					Position = new Vector2((float)mouseEventArgs.OffsetX, (float)mouseEventArgs.OffsetY)
				});
			}
		}

		public void MapOnTouchEnd(TouchEventArgs touchEventArgs)
		{
		}

		public void Dispose()
		{
			ClientService.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
			_ = InvokeAsync(StateHasChanged);

			if (ClientService.View?.Lobby?.World == null)
			{
				NavigationManager.NavigateTo("/lobby");
			}
		}
	}
}
