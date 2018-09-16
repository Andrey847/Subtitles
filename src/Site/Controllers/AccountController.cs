using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Site.Services.Identity;

namespace SubtitlesLearn.Site.Controllers
{
    public class AccountController : Controller
    {
		#region Fields

		private readonly ApplicationUserManager _userManager;
		private readonly SignInManager<Customer> _signInManager;

		private readonly RecaptchaSettings _recaptchaSettings;
		private IHttpContextAccessor _accessor;

		#endregion Fields

		#region Construction

		/// <summary>
		/// Main controller.
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		/// <param name="recaptchaSettings"></param>
		/// <param name="accessor"></param>
		public AccountController(
			ApplicationUserManager userManager,
			SignInManager<Customer> signInManager,
			IOptions<RecaptchaSettings> recaptchaSettings,
			IHttpContextAccessor accessor)
		{
			_userManager = userManager;
			_signInManager = signInManager;

			_recaptchaSettings = recaptchaSettings.Value;
			_accessor = accessor;
		}

		#endregion Cosntruction

		#region Methods
		
		public IActionResult Index()
        {
            return View();
        }

			   /// <summary>
			   /// Show login page
			   /// </summary>
			   /// <param name="returnUrl"></param>
			   /// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			// Clear the existing external cookie to ensure a clean login process			
			await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

			ViewData["ReturnUrl"] = returnUrl;
#warning Place for google auth.
			//ViewBag.GoogleAuthUrl = GoogleAuthManager.Instance.GetAuthUrl();

			ViewBag.Customer = new Customer();

			return View();
		}

		#endregion Methods
	}
}