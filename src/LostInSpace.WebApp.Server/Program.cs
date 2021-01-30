using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace LostInSpace.WebApp.Server
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
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
				}
			);
		}
	}
}
