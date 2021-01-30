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
	public partial class LobbyPage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService ClientService { get; set; }
		[Inject] protected NavigationManager NavigationManager { get; set; }
		[Inject] protected IJSRuntime JsRuntime { get; set; }

		protected override Task OnInitializedAsync()
		{
			ClientService.OnProcedureApplied += OnProcedureApplied;

			return Task.CompletedTask;
		}

		public void LaunchGameButton(MouseEventArgs mouseEventArgs)
		{
			_ = ClientService.SendCommandAsync(new LaunchGameCommand()
			{
			});
		}

		public void Dispose()
		{
			ClientService.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
			_ = InvokeAsync(StateHasChanged);

			if (ClientService.View?.Lobby?.World != null)
			{
				NavigationManager.NavigateTo("/navigate");
			}
		}
	}
}
