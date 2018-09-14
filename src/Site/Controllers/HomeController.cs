using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Models;
using System.Diagnostics;

namespace SubtitlesLearn.Site.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return RedirectToAction("Index", "WorkPlace");
		}

		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
