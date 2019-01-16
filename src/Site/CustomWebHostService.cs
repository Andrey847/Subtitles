using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SubtitlesLearn.Site
{
	/// <summary>
	/// Implements main win service methods as Start Stop etc.
	/// Can be used for correct system shutdown.
	/// </summary>
	public class CustomWebHostService : WebHostService
	{
		private ILogger _logger;

		/// <summary>
		/// Основной конструктор.
		/// </summary>
		/// <param name="host"></param>
		public CustomWebHostService(IWebHost host) : base(host)
		{
			_logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
		}

		/// <summary>
		/// <see cref="WebHostService.OnStarting(string[])"/>
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStarting(string[] args)
		{
			_logger.LogDebug("OnStarting method called.");
			base.OnStarting(args);
		}

		/// <summary>
		/// <see cref="WebHostService.OnStarted()"/>
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStarted()
		{
			_logger.LogDebug("OnStarted method called.");
			base.OnStarted();
		}

		/// <summary>
		/// <see cref="WebHostService.OnStopping()"/>
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStopping()
		{
			_logger.LogDebug("OnStopping method called.");			

			base.OnStopping();
		}
	}
}
