using Newtonsoft.Json;

namespace SubtitlesLearn.Logic.Infrastructure.GoogleAuth
{
	/// <summary>
	/// Token information from google.
	/// </summary>
	public class AuthToken
	{
		/// <summary>
		/// The token that your application sends to authorize a Google API request.
		/// </summary>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// A token that you can use to obtain a new access token. Refresh tokens are valid until the user revokes access. Again, this field is only present in this response if you set the access_type parameter to offline in the initial request to Google's authorization server.
		/// </summary>
		[JsonProperty("expires_in")]
		public string ExpiresIn { get; set; }

		/// <summary>
		/// The remaining lifetime of the access token in seconds.
		/// </summary>
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// The type of token returned. At this time, this field's value is always set to Bearer.
		/// </summary>
		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }
	}
}
