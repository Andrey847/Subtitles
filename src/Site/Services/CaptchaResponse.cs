using Newtonsoft.Json;
using System.Collections.Generic;

namespace SubtitlesLearn.Site.Services
{
	/// <summary>
	/// Recaptcha response.
	/// </summary>
	public class CaptchaResponse
	{
		/// <summary>
		/// Is success?
		/// </summary>
		[JsonProperty("success")]
		public bool Success { get; set; }

		/// <summary>
		/// Errors codes in case of error(s).
		/// </summary>
		[JsonProperty("error-codes")]
		public List<string> ErrorCodes { get; set; }
	}
}
