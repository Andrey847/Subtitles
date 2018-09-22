using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Site.Models;
using SubtitlesLearn.Site.Services.Identity;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Controllers
{
	/// <summary>
	/// Main authentication controller.
	/// </summary>
	public class AccountController : Controller
	{
		#region Constants

		private const string MSG_GREETING = "greeting";

		#endregion Constants

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
		[HttpGet("[controller]/[action]/{msg?}")]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string msg, string returnUrl = null)
		{
			ViewBag.ShowGreeting = msg == MSG_GREETING;

			// Clear the existing external cookie to ensure a clean login process			
			await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

			ViewData["ReturnUrl"] = returnUrl;
#warning Place for google auth.
			//ViewBag.GoogleAuthUrl = GoogleAuthManager.Instance.GetAuthUrl();

			ViewBag.Customer = new Customer();

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (ModelState.IsValid)
			{
				if (!string.IsNullOrEmpty(model.Email)
				&& !string.IsNullOrEmpty(model.Password))
				{
					// This doesn't count login failures towards account lockout
					// To enable password failures to trigger account lockout, set lockoutOnFailure: true
					var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

					if (result.Succeeded)
					{
						if (string.IsNullOrEmpty(returnUrl))
						{
							return RedirectToAction(nameof(WorkPlaceController.Index), "WorkPlaceController");
						}
						else
						{
							return Redirect(returnUrl);
						}
					}
					else
					{
						ModelState.AddModelError(string.Empty, "Login failed. Please try again.");
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Login and passwords must be populated.");
				}
			}

			if (!ModelState.IsValid)
			{
				await LogManager.Instance.LogInfo($"Failed login attempt", $"IP: {_accessor.HttpContext.Connection.RemoteIpAddress.ToString()}, login {model.Email}");
			}

			return View(model);
		}

		//
		// GET: /Account/Register
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
#warning Google auth.
			//ViewBag.GoogleAuthUrl = GoogleAuthManager.Instance.GetAuthUrl();			

			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (ModelState.IsValid)
			{
				Customer user = new Customer
				{
					Email = model.Email,
					IsConfirmed = false
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{					
					await LogManager.Instance.LogInfo($"User {model.Email} created a new account with password.");
					
					// Send him to login page with greeting message
					return RedirectToAction(nameof(AccountController.Login), "AccountController", MSG_GREETING);
				}				
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/Logout     
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			await LogManager.Instance.LogInfo($"User logged out");

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		#endregion Methods
	}
}