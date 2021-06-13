using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Procedures;
using PewPew.WebApp.Shared.View;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class LobbyPage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService? Client { get; set; }
		[Inject] protected NavigationManager? NavigationManager { get; set; }
		[Inject] protected IJSRuntime? JsRuntime { get; set; }

		private string? nameCache = null;

		public string YourName
		{
			get
			{
				EnsureValidState();

				if (nameCache != null)
				{
					return nameCache;
				}

				if (Client.View?.Client == null
					|| Client.View?.Lobby == null)
				{
					return "";
				}

				Client.View.Lobby.Players.TryGetValue(Client.View.Client.ClientId, out var result);
				return result?.DisplayName ?? "";
			}
			set
			{
				nameCache = value;
				UpdateDisplayName(value);
			}
		}

		public string YourClass
		{
			get
			{
				EnsureValidState();

				if (Client.View?.Client == null
					|| Client.View?.Lobby == null)
				{
					return ShipTypes.Scout;
				}

				Client.View.Lobby.Players.TryGetValue(Client.View.Client.ClientId, out var result);
				return result?.ShipClass ?? ShipTypes.Scout;
			}
			set
			{
				UpdateShipClass(value);
			}
		}

		public int YourTeam
		{
			get
			{
				EnsureValidState();

				if (Client.View?.Client == null
					|| Client.View?.Lobby == null)
				{
					return -1;
				}

				Client.View.Lobby.Players.TryGetValue(Client.View.Client.ClientId, out var result);
				return result?.TeamId ?? -1;
			}
		}

		protected override Task OnInitializedAsync()
		{
			EnsureValidState();

			if (Client.View?.Lobby == null)
			{
				NavigationManager.NavigateTo("/");
			}

			Client.OnProcedureApplied += OnProcedureApplied;

			return Task.CompletedTask;
		}

		public void UpdateDisplayName(string newDisplayName)
		{
			EnsureValidState();

			_ = Client.SendCommandAsync(new LobbyUpdateDisplayNameCommand()
			{
				DisplayName = newDisplayName
			});
		}

		public void UpdateShipClass(string newShipClass)
		{
			EnsureValidState();

			_ = Client.SendCommandAsync(new LobbyUpdateClassCommand()
			{
				ShipClass = newShipClass
			});
		}

		public void JoinTeamButton(int teamId)
		{
			EnsureValidState();

			_ = Client.SendCommandAsync(new LobbyUpdateTeamCommand()
			{
				NewTeamId = teamId
			});
		}

		public void LaunchGameButton(MouseEventArgs mouseEventArgs)
		{
			EnsureValidState();

			_ = Client.SendCommandAsync(new LaunchGameCommand()
			{
			});
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

			if (Client.View?.Lobby == null)
			{
				NavigationManager.NavigateTo("/");
			}

			if (Client.View?.Lobby?.World != null)
			{
				NavigationManager.NavigateTo("/navigate");
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
