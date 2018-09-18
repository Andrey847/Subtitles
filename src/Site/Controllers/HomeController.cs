using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Models;
using System.Diagnostics;

namespace SubtitlesLearn.Site.Controllers
{
	/// <summary>
	/// Simple greeting page.
	/// </summary>
	[AllowAnonymous]
	public class HomeController : Controller
	{
		/// <summary>
		/// Main greeting page.
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Error processing.
		/// </summary>
		/// <returns></returns>
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
