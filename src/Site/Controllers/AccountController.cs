using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using SubtitlesLearn.Logic.Infrastructure.GoogleAuth;
using SubtitlesLearn.Site.Models;
using SubtitlesLearn.Site.Services;
using SubtitlesLearn.Site.Services.Identity;
using System;
using System.Linq;
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
		/// Main constructor.
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

		#endregion Construction

		#region Methods

		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Restore password page.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Restore()
		{
			ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;
			return View(new RestorePasswordRequest());
		}

		/// <summary>
		/// Creates restore password link and sends related email to the customer.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Restore(RestorePasswordRequest request)
		{
			ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;

			bool verified = await CaptchaVerifier.VerifyAsync(_recaptchaSettings, Request);

			if (!verified)
			{
				ModelState.AddModelError(string.Empty, "Captcha failed.");
			}
			else if (string.IsNullOrEmpty(request.Email))
			{
				ModelState.AddModelError(string.Empty, "Email missing.");
			}
			else if (!await UserManager.Instance.RestorePassword(request))
			{
				ModelState.AddModelError(string.Empty, "Unable to send restore code.");
			}

			if (ModelState.IsValid)
			{
				return View("RestoreLinkSent");
			}
			else
			{
				return View(request);
			}
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
			ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;

			// Clear the existing external cookie to ensure a clean login process			
			await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

			ViewData["ReturnUrl"] = returnUrl;

			ViewBag.GoogleAuthUrl = GoogleAuthManager.Instance.GetAuthUrl();

			ViewBag.Customer = new Customer();

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				if (!string.IsNullOrEmpty(model.Email)
				&& !string.IsNullOrEmpty(model.Password))
				{
					model.Email = UserManager.Instance.UnifyGmail(model.Email);

					// This doesn't count login failures towards account lockout
					// To enable password failures to trigger account lockout, set lockoutOnFailure: true
					var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);

					if (result.Succeeded)
					{
						if (string.IsNullOrEmpty(returnUrl))
						{
							return RedirectToAction(nameof(WorkPlaceController.Index), "WorkPlace");
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
			ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;

			ViewBag.GoogleAuthUrl = GoogleAuthManager.Instance.GetAuthUrl();

			return View();
		}

		/// <summary>
		/// Changes password (after restore or simple password change).
		/// </summary>
		/// <param name="restorePasswordCode"></param>
		/// <returns></returns>
		[HttpGet("[controller]/[action]/{restorePasswordCode?}")]
		[AllowAnonymous]
		public async Task<IActionResult> ChangePassword(string restorePasswordCode)
		{
			Customer customer = await _userManager.GetUserAsync(User);
			bool failed = false;
			ChangePasswordViewModel model = new ChangePasswordViewModel();

			if (!string.IsNullOrEmpty(restorePasswordCode))
			{
				string email = await UserManager.Instance.VerifyPasswordRestore(restorePasswordCode);
				if (string.IsNullOrEmpty(email))
				{
					failed = true;
				}
				else
				{
					model.Email = email;
				}
			}
			else if (customer == null)
			{
				// no restore code and no registered customer. Show error page. (unreal case).
				failed = true;
			}
			else
			{
				model.Email = customer.Email;
			}

			if (failed)
			{
				return View("WrongRestorePasswordCode");
			}
			else
			{
				model.RestorePasswordCode = restorePasswordCode;
				return View(model);
			}
		}

		[HttpPost("[controller]/[action]/{restorePasswordCode?}")]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			Customer customer = await _userManager.GetUserAsync(User);

			if (ModelState.IsValid)
			{
				// there are 2 cases possible. Current user changes the password or it is restoring password options. So we have to restore user from password restore code.
				if (customer == null)
				{
					customer = await UserManager.Instance.GetUser(model.Email);
				}

				if (model.Password != model.PasswordConfirmation)
				{
					ModelState.AddModelError(string.Empty, "Passwords do not match.");
				}
				else if (string.IsNullOrEmpty(model.RestorePasswordCode))
				{
					// Verify old password
					if (_userManager.PasswordHasher.VerifyHashedPassword(customer, customer.PasswordHash, model.OldPassword) != PasswordVerificationResult.Success)
					{
						ModelState.AddModelError(string.Empty, "Entered wrong old password");
					}
				}
				else
				{
					// Verify restore code
					if (model.RestorePasswordCode != customer.RestorePasswordCode)
					{
						// it is very strange! immidiately redirect user to login page.
						await LogManager.Instance.LogError("Wrong restore password code", $"Customer: {customer.Email} ({customer.Id})");
						return RedirectToAction("Login", "Account");
					}
				}
			}

			if (ModelState.IsValid)
			{
				// Great! change the password 
				await _userManager.ResetPasswordAsync(customer, string.Empty, model.Password);

				var loginResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, lockoutOnFailure: false);
				await LogManager.Instance.LogInfo("Login after change the password", $"Login={model.Email}; State={loginResult.Succeeded}");

				// and redirect to main page.
				return RedirectToAction("Index", "WorkPlace");
			}
			else
			{
				// Here are errors, so show them.
				return View(model);
			}
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			ViewData["RecaptchaSiteKey"] = _recaptchaSettings.SiteKey;

			bool verified = await CaptchaVerifier.VerifyAsync(_recaptchaSettings, Request);

			if (!verified)
			{
				ModelState.AddModelError(string.Empty, "Captcha failed");
			}

			if (ModelState.IsValid)
			{
				Customer user = new Customer
				{
					Email = UserManager.Instance.UnifyGmail(model.Email),
					IsConfirmed = false
				};
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await LogManager.Instance.LogInfo($"User {model.Email} created a new account with password.");

					// Send him to login page with greeting message
					return Redirect($"/Account/{nameof(AccountController.Login)}/{MSG_GREETING}");
				}
				else
				{
					ModelState.AddModelError(string.Empty, result.Errors.First().Description);
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

		/// <summary>
		/// Google auth callback processing.
		/// </summary>
		/// <returns></returns>
		[AllowAnonymous]
		[HttpGet("[controller]/signin-google")]
		public async Task<IActionResult> SigninGoogle([FromQuery] string code)
		{
			await LogManager.Instance.LogInfo("Google auth callback", $"Code = {code}");

			try
			{
				AuthToken token = await GoogleAuthManager.Instance.GetToken(code);
				await LogManager.Instance.LogDebug("Google auth callback", $"Token = {token.AccessToken}");

				UserInfo info = await GoogleAuthManager.Instance.GetUserInfo(token.AccessToken);
				info.Email = UserManager.Instance.UnifyGmail(info.Email);

				await LogManager.Instance.LogDebug("Google auth callback info", $"Email: {info.Email}, Name: {info.Name}");
				
				// now - login.
				Customer customer = await UserManager.Instance.GetUser(info.Email);

				if (customer == null)
				{
					// create new login
					customer = new Customer();
					customer.Email = info.Email;
					customer.IsConfirmed = true;

					await _userManager.CreateAsync(customer);

					// to get customer Id or next signId doesn't work.
					customer = await UserManager.Instance.GetUser(info.Email);
				}


				// and sign in now.
				await _signInManager.SignInAsync(customer, true);

				// the main page
				return Redirect("/");
			}
			catch (Exception ex)
			{
				await LogManager.Instance.LogError("Unable to log by google.", ex);

				return RedirectToAction("Login");
			}
		}

		/// <summary>
		/// Confirms account and if success, autologins. 
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		[HttpGet("[controller]/[action]/{code}")]
		public async Task<IActionResult> Confirm(string code)
		{
			string email = await UserManager.Instance.Unblock(code);

			if (!string.IsNullOrWhiteSpace(email))			
			{
				// auto login and go to workplace
				Customer customer = await UserManager.Instance.GetUser(email);
				await _signInManager.SignInAsync(customer, true);				
			}

			return Redirect("/");
		}

		#endregion Methods
	}
}