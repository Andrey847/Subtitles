namespace SubtitlesLearn.Logic.Infrastructure.GoogleAuth
{
	/// <summary>
	/// Settings for google authentication.
	/// </summary>
	public class GoogleAuthSettings
	{
		/// <summary>
		/// The main auth url.
		/// </summary>
		public string Url { get; set; } = "https://accounts.google.com/o/oauth2/auth";

		/// <summary>
		/// Client Id from google console.
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// Client secret from google console.
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		/// URL to site controlled that receives callback from google.
		/// </summary>
		public string RedirectUrl { get; set; }

		/// <summary>
		/// A space-delimited list of scopes that identify the resources that your application could access on the user's behalf. These values inform the consent screen that Google displays to the user.
		/// </summary>
		public string Scopes { get; set; } = "https://www.googleapis.com/auth/userinfo.email";

		/// <summary>
		/// Url for token getting.
		/// </summary>
		public string TokenUri { get; set; } = "https://accounts.google.com/o/oauth2/token";

		/// <summary>
		/// Url to get user information by token.
		/// </summary>
		public string UserInfoUrl { get; set; } = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=";
	}
}
