using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site
{
	/// <summary>
	/// Win service .Net core wrapper.
	/// </summary>
	public static class WebHostServiceExtensions
	{
		public static void RunAsCustomService(this IWebHost host)
		{
			var webHostService = new CustomWebHostService(host);
			ServiceBase.Run(webHostService);
		}
	}
}
