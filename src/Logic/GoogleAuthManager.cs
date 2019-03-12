using Newtonsoft.Json;
using SubtitlesLearn.Logic.Infrastructure;
using SubtitlesLearn.Logic.Infrastructure.GoogleAuth;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SubtitlesLearn.Logic
{
	public class GoogleAuthManager : ManagerBase
	{
		#region Properties

		/// <summary>
		/// Settings for google auth serivces.
		/// </summary>
		public GoogleAuthSettings Settings { get; set; } = new GoogleAuthSettings();

		#endregion Properties

		#region Singleton

		private static Lazy<GoogleAuthManager> _instance = new Lazy<GoogleAuthManager>(() => new GoogleAuthManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static GoogleAuthManager Instance => _instance.Value;
		
		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private GoogleAuthManager()
		{

		}

		#endregion Singleton

		#region Methods

		/// <summary>
		/// Returns the main URL to place on UI for authentication.
		/// </summary>
		/// <param name="redirectDetails">This parameter is used to customize URL, that google invokes. Must strart with /</param>
		/// <returns></returns>
		public string GetAuthUrl(string redirectDetails = "")
		{
			if (Settings == null || string.IsNullOrEmpty(Settings.Url))
			{
				throw new InvalidOperationException("Settings are not initialized.");
			}

			return $"{Settings.Url}?client_id={Settings.ClientId}&redirect_uri={HttpUtility.UrlEncode(Settings.RedirectUrl + redirectDetails)}&scope={HttpUtility.UrlEncode(Settings.Scopes)}&response_type=code";
		}

		/// <summary>
		/// Gets token information from google;
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public async Task<AuthToken> GetToken(string code, string redirectDetails = "")
		{
			string postData = "code=" + code + "&client_id=" + Settings.ClientId + "&client_secret=" + Settings.ClientSecret + "&redirect_uri=" + Settings.RedirectUrl + redirectDetails + "&grant_type=authorization_code";
			byte[] postDataAsBytes = Encoding.UTF8.GetBytes(postData);

			using (WebClient client = new WebClient())
			{
				client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
				string json = await client.UploadStringTaskAsync(Settings.TokenUri, "POST", postData);
				return JsonConvert.DeserializeObject<AuthToken>(json);
			}
		}

		/// <summary>
		/// Returns user info by token.
		/// </summary>
		/// <returns></returns>
		public async Task<UserInfo> GetUserInfo(string token)
		{
			string url = Settings.UserInfoUrl + "?access_token=" + token;

			try
			{
				using (WebClient client = new WebClient())
				{
					client.Headers["Content-Type"] = "application/x-www-form-urlencoded";
					string json = await client.DownloadStringTaskAsync(url);
					return JsonConvert.DeserializeObject<UserInfo>(json);
				}
			}
			catch (Exception ex)
			{
				await Log.LogError(url, ex);				
				throw;
			}
		}

		#endregion Methods
	}
}
