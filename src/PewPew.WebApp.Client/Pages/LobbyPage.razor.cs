using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using PewPew.WebApp.Client.Services;
using PewPew.WebApp.Shared.Commands;
using PewPew.WebApp.Shared.Procedures;
using PewPew.WebApp.Shared.View;
using System;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client.Pages
{
	public partial class LobbyPage : ComponentBase, IDisposable
	{
		[Inject] protected ClientService Client { get; set; }
		[Inject] protected NavigationManager NavigationManager { get; set; }
		[Inject] protected IJSRuntime JsRuntime { get; set; }

		private string nameCache = null;

		public string YourName
		{
			get
			{
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
				if (Client.View?.Client == null
					|| Client.View?.Lobby == null)
				{
					return ShipTypes.Scout;
				}

				Client.View.Lobby.Players.TryGetValue(Client.View.Client.ClientId, out var result);
				return result.ShipClass ?? ShipTypes.Scout;
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
			if (Client.View?.Lobby == null)
			{
				NavigationManager.NavigateTo("/");
			}

			Client.OnProcedureApplied += OnProcedureApplied;

			return Task.CompletedTask;
		}

		public void UpdateDisplayName(string newDisplayName)
		{
			_ = Client.SendCommandAsync(new LobbyUpdateDisplayNameCommand()
			{
				DisplayName = newDisplayName
			});
		}

		public void UpdateShipClass(string newShipClass)
		{
			_ = Client.SendCommandAsync(new LobbyUpdateClassCommand()
			{
				ShipClass = newShipClass
			});
		}

		public void JoinTeamButton(int teamId)
		{
			_ = Client.SendCommandAsync(new LobbyUpdateTeamCommand()
			{
				NewTeamId = teamId
			});
		}

		public void LaunchGameButton(MouseEventArgs mouseEventArgs)
		{
			_ = Client.SendCommandAsync(new LaunchGameCommand()
			{
			});
		}

		public void Dispose()
		{
			Client.OnProcedureApplied -= OnProcedureApplied;
		}

		public void OnProcedureApplied(NetworkedViewProcedure networkedViewProcedure)
		{
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
	}
}
