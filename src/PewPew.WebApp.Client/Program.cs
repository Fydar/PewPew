using HuskyNet.WebClient.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PewPew.WebApp.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = CreateHostBuilder(args);

			var host = builder.Build();

			await host.RunAsync();
		}

		private static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);

			builder.RootComponents.Add<App>("#app");

			builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			ConfigureServices(builder.Services);

			return builder;
		}

		private static void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<ClientService>();
		}
	}
}
