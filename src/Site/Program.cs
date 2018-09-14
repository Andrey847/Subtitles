using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SubtitlesLearn.Site
{
	/// <summary>
	/// Main entry point to the app.
	/// </summary>
	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args)
		{
			bool isService = true;
			if (Debugger.IsAttached || args.Contains("--console"))
			{
				isService = false;
			}

			var pathToContentRoot = Directory.GetCurrentDirectory();
			if (isService)
			{
				var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
				pathToContentRoot = Path.GetDirectoryName(pathToExe);
			}

			var config = new ConfigurationBuilder()
			.SetBasePath(pathToContentRoot)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build();

			return WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseUrls(config.GetChildren().ToArray().Where(item => item.Key == "server.urls").First().Value)
				.UseConfiguration(config)
				.UseHttpSys(options =>
				{
					options.Authentication.Schemes = Microsoft.AspNetCore.Server.HttpSys.AuthenticationSchemes.None;
					options.Authentication.AllowAnonymous = true;
				})
				.Build();
		}
	}
}
