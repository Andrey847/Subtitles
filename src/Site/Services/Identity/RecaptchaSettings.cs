namespace SubtitlesLearn.Site.Services.Identity
{
	/// <summary>
	/// Google recaptcha settings.
	/// </summary>
	public class RecaptchaSettings
	{
		/// <summary>
		/// Google server URL to check the captcha.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// Site key.
		/// </summary>
		public string SiteKey { get; set; }

		/// <summary>
		/// Private key.
		/// </summary>
		public string SecretKey { get; set; }
	}
}
