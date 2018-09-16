using Newtonsoft.Json;
using System.Collections.Generic;

namespace SubtitlesLearn.Site.Services.Identity
{
	/// <summary>
	/// Recaptcha response.
	/// </summary>
	public class CaptchaResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }

		[JsonProperty("error-codes")]
		public List<string> ErrorCodes { get; set; }
	}
}
