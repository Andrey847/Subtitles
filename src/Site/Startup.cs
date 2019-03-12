using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Site.Services;
using SubtitlesLearn.Site.Services.Identity;
using System;
using System.Threading.Tasks;

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
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
		   .SetBasePath(env.ContentRootPath)
		   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
		   .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
		   .AddJsonFile($"personal.json", optional: true, reloadOnChange: true)
		   .AddEnvironmentVariables();

			Configuration = builder.Build();

			DbHelper.ConnectionString = Configuration.GetConnectionString("Default");

			// Setup cardcom settings.
			EmailSettings emailSettings = new EmailSettings();
			Configuration.GetSection("Email").Bind(emailSettings);
			EmailManager.Instance.Settings = emailSettings;
			EmailManager.Instance.ManagerEmail = Configuration["ManagerEmail"];

			EmailManager.Instance.GlobalSettings
				= UserManager.Instance.GlobalSettings
				= SrtManager.Instance.GlobalSettings
				= SoundManager.Instance.GlobalSettings
					= new GlobalSettings()
					{
						BasePath = env.ContentRootPath,
						GetFullUrl = item => { return Configuration["DirectServerUrl"] + item; }
					};

			EmailManager.Instance.Log
				= UserManager.Instance.Log
				= SrtManager.Instance.Log
				= SoundManager.Instance.Log
				= LogManager.Instance;

			// Google auth settings
			Configuration.GetSection("GoogleAuth").Bind(GoogleAuthManager.Instance.Settings);

			UserManager.Instance.EmailNotifier = EmailManager.Instance;
			LogManager.Instance.LogInfo("Application started").GetAwaiter().GetResult();
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors(options =>
			{
				options.AddPolicy("AllowSpecificOrigin",
					builder => builder.WithOrigins("*")
						.AllowCredentials()
						.AllowAnyMethod()
						.AllowAnyHeader()
					);
			});

			services.AddOptions();
			services.Configure<RecaptchaSettings>(Configuration.GetSection("Recaptcha"));

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

			services.ConfigureApplicationCookie(options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromDays(1); // 1 day timeout of inactivity.
				options.SlidingExpiration = true;
			});

			services.Configure<SecurityStampValidatorOptions>(options =>
			{
				// one year for security validation.
				options.ValidationInterval = TimeSpan.FromDays(365);
			});

			services.AddSignalR();

			// Add framework services.
			services.AddMvc(config =>
			{
				config.Filters.Add(typeof(GlobalExceptionFilter));
			});

			LogManager.Instance.Error += SystemError;
		}

		private void SystemError(object sender, Logic.Infrastructure.ErrorArgs e)
		{
			// do that in parallel to do not block main thread
			Task.Run(() => EmailManager.Instance.NotifyAdmin("Application error", $"{e.Message}<br>Details:<br>{e.Details}"));
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStatusCodePagesWithRedirects("/");
			app.UseAuthentication();

			app.UseCors("AllowSpecificOrigin");

			app.UseStaticFiles();

			app.UseSignalR(route =>
			{
				route.MapHub<NotificationHub>("/NotificationHub");
			});

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
