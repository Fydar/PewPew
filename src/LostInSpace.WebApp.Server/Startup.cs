using HuskyNet.Instance.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LostInSpace.WebApp.Server
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<InstanceManagerService>();

			services.AddControllers();

			services.AddControllersWithViews();
			services.AddRazorPages();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
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
