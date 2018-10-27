using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Site.Models;
using SubtitlesLearn.Site.Services.Identity;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Controllers
{
	/// <summary>
	/// Main work place.
	/// </summary>
	[Authorize]
    public class SettingsController : Controller
    {
		#region Fields

		private readonly ApplicationUserManager _userManager;

		#endregion Fields

		#region Construction

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="userManager"></param>
		public SettingsController(ApplicationUserManager userManager)
		{
			_userManager = userManager;			
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Main page.
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index()
		{
			Customer customer = await _userManager.GetUserAsync(User);

			ViewBag.CurrentPageType = PageType.Settings;
			ViewBag.Languages = await SrtManager.Instance.GetLanguages();

			return View();
		}

		/// <summary>
		/// Return user settings.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<JsonResult> GetSettings()
		{
			Customer customer = await _userManager.GetUserAsync(User);

			CustomerSettings settings = await UserManager.Instance.GetSettings(customer.Id);

			return new JsonResult(settings);
		}

		/// <summary>
		/// Saves user settings.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		[HttpPost()]
		public async Task<IActionResult> UpdateSettings([FromBody] CustomerSettings settings)
		{
			await UserManager.Instance.UpdateSettings(settings);

			return Ok();
		}

		#endregion Methods
	}
}