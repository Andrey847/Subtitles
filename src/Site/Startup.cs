using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Site.Services.Identity;

namespace SubtitlesLearn.Site
{
	/// <summary>
	/// Initial file of the site.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Main construction.
		/// </summary>
		/// <param name="configuration"></param>
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddIdentity<Customer, CustomerRole>()
				.AddUserManager<ApplicationUserManager>()
				.AddRoleManager<ApplicationRoleManager>()
				.AddDefaultTokenProviders()
				.AddUserStore<CustomerUserStore>()
				.AddRoleStore<RoleStore>();
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

			services.AddResponseCompression(options =>
			{
				options.EnableForHttps = true;
			});

			services.Configure<IdentityOptions>(configureOptions =>
			{
				// Password settings.
				configureOptions.Password.RequireDigit = false;
				configureOptions.Password.RequiredLength = 1;
				configureOptions.Password.RequireNonAlphanumeric = false;
				configureOptions.Password.RequireUppercase = false;
				configureOptions.Password.RequireLowercase = false;
			});

			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			DbAccess.ConnectionString = "Data Source=(local)\\SQLEXPRESS;Initial Catalog=SubtitlesLearn;Integrated Security=True";

			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
