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

		public LocalId? CurrentlySelected { get; set; }

		private string InputDebug { get; set; }

		public enum NavigateInputMode
		{
			None,
			SetDestination,

		}

		protected override Task OnInitializedAsync()
		{
			ClientService.OnProcedureApplied += OnProcedureApplied;
			return Task.CompletedTask;
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender)
			{
				await JsRuntime.InvokeAsync<object>("PanAndZoom.Start");
			}
		}

		public void MapOnClick(MouseEventArgs mouseEventArgs)
		{
			InputDebug = $"{mouseEventArgs.OffsetX} {mouseEventArgs.OffsetY}";

			_ = ClientService.SendCommandAsync(new SetDestinationPositionCommand()
			{
				Position = new Vector2((float)mouseEventArgs.OffsetX, (float)mouseEventArgs.OffsetY)
			});
		}

		public void Dispose()
		{
			ClientService.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
			_ = InvokeAsync(StateHasChanged);
		}
	}
}
