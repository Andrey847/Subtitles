using Newtonsoft.Json;

namespace SubtitlesLearn.Logic.Infrastructure.GoogleAuth
{
	/// <summary>
	/// Aux class to get user info from google API.
	/// </summary>
	public class UserInfo
	{
		/// <summary>
		/// User email.
		/// </summary>
		[JsonProperty("email")]
		public string Email { get; set; }

		/// <summary>
		/// User name (full name).
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}
