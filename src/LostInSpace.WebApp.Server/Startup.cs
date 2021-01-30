using HuskyNet.Instance.Server.Services;
using LettuceEncrypt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace LostInSpace.WebApp.Server
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public IWebHostEnvironment Environment { get; }

		public Startup(IConfiguration configuration, IWebHostEnvironment environment)
		{
			Configuration = configuration;
			Environment = environment;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHealthChecks();

			services.AddSingleton<InstanceManagerService>();

			services.AddControllers();

			services.AddControllersWithViews();
			services.AddRazorPages();

			if (!Environment.IsDevelopment())
			{
				string domainName = Configuration.GetValue<string>("DOMAINNAME");

				services.AddLettuceEncrypt(options =>
				{
					options.AcceptTermsOfService = true;
					options.DomainNames = new string[] { domainName };
					options.EmailAddress = "dev.anthonymarmont@gmail.com";
				})
					.PersistDataToDirectory(new DirectoryInfo("lettuce"), null);
			}
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseHealthChecks("/api/health");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseWebSockets();

			app.UseEndpoints(endpoints =>
			{
				//endpoints.MapControllers();
				endpoints.MapControllerRoute(
				   name: "default",
				   pattern: "{controller=Home}/{action=Index}/{id?}");

				endpoints.MapRazorPages();
				endpoints.MapFallbackToFile("index.html");
			});
		}
	}
}
