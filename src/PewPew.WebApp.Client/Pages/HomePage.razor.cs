using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Procedures;
using System;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class HomePage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService Client { get; set; }
		[Inject] protected NavigationManager NavigationManager { get; set; }
		[Inject] protected IJSRuntime JsRuntime { get; set; }

		private string DirectJoinLobbyKey { get; set; }

		protected override async Task OnInitializedAsync()
		{
			Client.OnProcedureApplied += OnProcedureApplied;

			if (Client.View.Lobby != null)
			{
				await Client.SendCommandAsync(new LobbyLeaveCommand());
				Client.View.Lobby = null;
			}
		}

		public void JoinLobbyButton(string lobbyKey)
		{
			_ = Client.SendCommandAsync(new FrontendJoinLobbyCommand()
			{
				LobbyKey = lobbyKey
			});
		}

		public void DirectJoinLobbyButton(MouseEventArgs mouseEventArgs)
		{
			if (string.IsNullOrWhiteSpace(DirectJoinLobbyKey))
			{
				return;
			}

			_ = Client.SendCommandAsync(new FrontendJoinLobbyCommand()
			{
				LobbyKey = DirectJoinLobbyKey.Trim()
			});
		}

		public void CreateLobbyButton(MouseEventArgs mouseEventArgs)
		{
			_ = Client.SendCommandAsync(new FrontendCreateLobbyCommand());
		}

		public void Dispose()
		{
			Client.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
			_ = InvokeAsync(StateHasChanged);

			if (Client.View?.Lobby != null)
			{
				NavigationManager.NavigateTo("/lobby");
			}
		}
	}
}
