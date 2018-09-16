using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Services.Identity
{
	/// <summary>
	/// Aux captcha class.
	/// </summary>
	public class CaptchaVerifier
	{
		/// <summary>
		/// Captcha settings.
		/// </summary>
		private readonly RecaptchaSettings _settings;

		/// <summary>
		/// The main constructor.
		/// </summary>
		/// <param name="settings">Captcha settings.</param>
		public CaptchaVerifier(RecaptchaSettings settings)
		{
			_settings = settings;
		}

		/// <summary>
		/// Verifies captcha.
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		public static async Task<bool> VerifyAsync(RecaptchaSettings settings, HttpRequest request)
		{
			CaptchaVerifier verifier = new CaptchaVerifier(settings);
			return await verifier.VerifyAsync(request);
		}

		/// <summary>
		/// Verifies captcha.
		/// </summary>
		/// <returns>True - ok, False - failed.</returns>
		public async Task<bool> VerifyAsync(HttpRequest request)
		{
			string captchaImage = request.Form["g-recaptcha-response"].ToString();
			if (string.IsNullOrEmpty(captchaImage))
				return false;

			string postData = $"&secret={_settings.SecretKey}&remoteip={request.HttpContext.Connection.RemoteIpAddress}&response={captchaImage}";
			byte[] postDataAsBytes = Encoding.UTF8.GetBytes(postData);

			using (WebClient client = new WebClient())
			{
				client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
				string json = await client.UploadStringTaskAsync(_settings.Url, "POST", postData);
				return JsonConvert.DeserializeObject<CaptchaResponse>(json)?.Success ?? false;
			}
		}
	}
}
