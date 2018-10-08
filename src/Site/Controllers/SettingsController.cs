using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
using SubtitlesLearn.Site.Models;
using SubtitlesLearn.Site.Services.Identity;

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

			ViewBag.CurrentPageType = PageType.WorkPlace;
			ViewBag.Languages = await SrtManager.Instance.GetLanguages();
			
			return View();
		}

		#endregion Methods
	}
}