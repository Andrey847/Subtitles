﻿using System;
using System.Threading.Tasks;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;

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

				await EmailManager.Instance.SendSimpleText(user.Email, "Subtitles Learn: confirmation",
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

		#endregion Methods
	}
}
