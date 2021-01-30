using HuskyNet.WebClient.Services;
using LostInSpace.WebApp.Shared.Procedures;
using Microsoft.AspNetCore.Components;
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

		protected override async Task OnInitializedAsync()
		{
			ClientService.OnProcedureApplied += OnProcedureApplied;
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await JsRuntime.InvokeAsync<object>("PanAndZoom.Start");
			}
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
