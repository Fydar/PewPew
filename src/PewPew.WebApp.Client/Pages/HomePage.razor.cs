using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Procedures;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class HomePage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService? Client { get; set; }
		[Inject] protected NavigationManager? NavigationManager { get; set; }
		[Inject] protected IJSRuntime? JsRuntime { get; set; }

		private string? DirectJoinLobbyKey { get; set; }

		protected override async Task OnInitializedAsync()
		{
			EnsureValidState();

			Client.OnProcedureApplied += OnProcedureApplied;

			if (Client.View?.Lobby != null)
			{
				await Client.SendCommandAsync(new LobbyLeaveCommand());
				Client.View.Lobby = null;
			}
		}

		public void JoinLobbyButton(string lobbyKey)
		{
			EnsureValidState();

			_ = Client.SendCommandAsync(new FrontendJoinLobbyCommand()
			{
				LobbyKey = lobbyKey
			});
		}

		public void DirectJoinLobbyButton(MouseEventArgs mouseEventArgs)
		{
			EnsureValidState();

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
			EnsureValidState();

			_ = Client.SendCommandAsync(new FrontendCreateLobbyCommand());
		}

		public void Dispose()
		{
			EnsureValidState();

			Client.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure? networkedViewProcedure)
		{
			EnsureValidState();

			_ = InvokeAsync(StateHasChanged);

			if (Client.View?.Lobby != null)
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
