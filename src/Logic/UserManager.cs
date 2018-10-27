using System;
using System.Threading.Tasks;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Controls all operations with user.
	/// </summary>
	public class UserManager : ManagerBase
	{
		#region Singleton

		private static Lazy<UserManager> _instance = new Lazy<UserManager>(() => new UserManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static UserManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private UserManager()
		{

		}

		#endregion Singleton

		#region Methods

		/// <summary>
		/// User creation.
		/// </summary>
		/// <param name="user"></param>
		public async Task CreateUser(Customer user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			if (string.IsNullOrWhiteSpace(user.Email))
				throw new ArgumentException("Email is empty");

			if (string.IsNullOrWhiteSpace(user.PasswordHash))
				throw new ArgumentException("Password hash is empty.");

			await UserAccess.CreateUser(user);

			if (!user.IsConfirmed)
			{
				// For non-google users send the confirmation notification to activate account.
				user.ConfirmationCode = Guid.NewGuid().ToString();

				// Customner is not confirmed. do it!
				string url = GlobalSettings.GetFullUrl($"/Account/Confirm/{user.ConfirmationCode}");

				await EmailNotifier.SendSimpleText(user.Email, "Subtitles Learn: Confirmation",
					@"
					<h3>Please click on this link to activate your account</h3>
					<a href='" + url + @"'>" + url + @"</a>
					");

				await Log.LogInfo("Account confirmation code was sent", $"Email = {user.Email}, code = {user.ConfirmationCode}");
			}

			await Log.LogInfo("User created", $"Email: {user.Email}");
		}

		/// <summary>
		/// Returns Customer by its email.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public async Task<Customer> GetUser(string email)
		{
			return await UserAccess.GetUser(email: email);
		}

		/// <summary>
		/// Returns Customer by its ID.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public async Task<Customer> GetUser(int customerId)
		{
			return await UserAccess.GetUser(customerId: customerId);
		}

		/// <summary>
		/// Changes password for the user.
		/// </summary>
		/// <param name="user"></param>
		public async Task ChangePassword(Customer user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			await UserAccess.UpdatePassword(user);

			await Log.LogInfo("Customer password changed", $"Customer: {user.Email}");
		}

		/// <summary>
		/// Sends email notification to the Customer in order to restore the password.
		/// 
		/// returns true - email was sent. false - email was not found at all.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public async Task<bool> RestorePassword(RestorePasswordRequest request)
		{
			await Log.LogInfo("Restore password request", $"Email = {request.Email}");
			
			string code = await UserAccess.GetRestoreCode(request.Email);
			bool result;

			if (string.IsNullOrEmpty(code))
			{
				result = false;
				await Log.LogInfo("Unable to create restore code", $"Email = {request.Email}");				
			}
			else
			{
				string url = GlobalSettings.GetFullUrl($"/Account/ChangePassword/{code}");

				result = await EmailNotifier.SendSimpleText(request.Email, "Subtitles Learn: Restore password",
					@"
					<h3>Please click the link to change your password</h3>
					<a href='" + url + @"'>" + url + @"</a>
					");

				await Log.LogInfo("Restore password code was sent", $"Email = {request.Email}");
			}

			return result;
		}

		/// <summary>
		/// Verifies restore password code and returns related customer email if everything is ok.
		/// </summary>
		/// <param name="restorePasswordCode"></param>
		/// <returns></returns>
		public async Task<string> VerifyPasswordRestore(string restorePasswordCode)
		{
			if (string.IsNullOrEmpty(restorePasswordCode))
				throw new ArgumentNullException();

			return await UserAccess.VerifyPasswordRestore(restorePasswordCode);
		}

		/// <summary>
		/// Returns current settings.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public async Task<CustomerSettings> GetSettings(int customerId)
		{
			return await UserAccess.GetSettings(customerId);
		}

		/// <summary>
		/// Updates (creates or updates) settings for customer.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public async Task UpdateSettings(CustomerSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			await Log.LogInfo("Settings updated", $"Customer Id: {settings.CustomerId}");

			await UserAccess.UpdateSettings(settings);
		}

		#endregion Methods
	}
}
