using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SubtitlesLearn.Site
{
	/// <summary>
	/// Main entry point to the app.
	/// </summary>
	public class Program
	{


		public static void Main(string[] args)
		{
			RunWebHost(args);
		}

		public static void RunWebHost(string[] args)
		{
			bool isService = true;
			if (Debugger.IsAttached || args.Contains("--console"))
			{
				isService = false;
			}

			string pathToContentRoot = Directory.GetCurrentDirectory();
			if (isService)
			{
				string pathToExe = Process.GetCurrentProcess().MainModule.FileName;
				pathToContentRoot = Path.GetDirectoryName(pathToExe);
			}

			IConfigurationRoot config = new ConfigurationBuilder()
				.SetBasePath(pathToContentRoot)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			IWebHost host = WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseUrls(config.GetChildren().ToArray().Where(item => item.Key == "server.urls").First().Value)
				.UseConfiguration(config)
				.UseContentRoot(pathToContentRoot)
				.UseHttpSys(options =>
				{
					options.Authentication.Schemes = Microsoft.AspNetCore.Server.HttpSys.AuthenticationSchemes.None;
					options.Authentication.AllowAnonymous = true;
				})
				.Build();

			if (isService)
			{
				host.RunAsCustomService();
			}
			else
			{
				host.Run();
			}
		}
	}
}
