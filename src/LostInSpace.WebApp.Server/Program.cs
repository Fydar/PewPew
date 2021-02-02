using LostInSpace.WebApp.Server.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Net;

namespace LostInSpace.WebApp.Server
{
	public class Program
	{
		public static int Main(string[] args)
		{
			var loggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel", LogEventLevel.Error)
				.Enrich.FromLogContext()
				.WriteTo.Sink(new ColoredConsoleSink());

			var logger = loggerConfiguration.CreateLogger();
			Log.Logger = logger;

			try
			{
				var host = CreateHostBuilder(args).Build();
				host.Start();

				var serverFeatures = host.Services.GetRequiredService<IServer>().Features;
				var addresses = serverFeatures.Get<IServerAddressesFeature>().Addresses;

				Log.Information($"Web host started listening on {string.Join(", ", addresses)}");

				host.WaitForShutdown();

				return 0;
			}
			catch (Exception exception)
			{
				Log.Fatal(exception, "Host terminated unexpectedly");
				return 1;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.ConfigureHostConfiguration(config =>
				{
					config.AddCommandLine(args);
					config.AddEnvironmentVariables("CONFIG_");
				})
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();

					webBuilder.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True");

					if (webBuilder.GetSetting("Environment") != "Development")
					{
						webBuilder.UseKestrel(kestral =>
						{
							var appServices = kestral.ApplicationServices;

							kestral.Listen(IPAddress.Any, 80);

							kestral.Listen(
								IPAddress.Any, 443,
								listen => listen.UseHttps(adapter =>
								{
									adapter.UseLettuceEncrypt(appServices);
								}));
						});
					}
				})
				.UseSerilog();
		}
	}
}
