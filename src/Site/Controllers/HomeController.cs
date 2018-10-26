using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Models;
using SubtitlesLearn.Site.Services.Identity;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Controllers
{
	/// <summary>
	/// Simple greeting page.
	/// </summary>
	[AllowAnonymous]
	public class HomeController : Controller
	{
		private readonly ApplicationUserManager _userManager;

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="userManager"></param>
		public HomeController(ApplicationUserManager userManager)
		{
			_userManager = userManager;
		}

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
			return View(new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			});
		}

		/// <summary>
		/// Sends contact message from user.
		/// </summary>
		/// <returns></returns>
		[HttpPost("[controller]/[action]")]
		[Authorize]
		public async Task<IActionResult> SendMessage([FromForm]string message)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			await EmailManager.Instance.SendSimpleText(EmailManager.Instance.ManagerEmail,
				$"Sub-learn message: from {customer.Email}",
				message);

			return Ok();
		}
	}
}
