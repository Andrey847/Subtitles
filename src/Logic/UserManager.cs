using System;
using SubtitlesLearn.Logic.Entities;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Controls all operations with user.
	/// </summary>
	public class UserManager
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

		public void CreateUser(Customer user)
		{
			throw new NotImplementedException();
		}

		public Customer GetUser(string normalizedUserName)
		{
			throw new NotImplementedException();
		}

		public Customer GetUser(int userId)
		{
			throw new NotImplementedException();
		}

		public void ChangePassword(Customer user)
		{
			throw new NotImplementedException();
		}

		#endregion Singleton
	}
}
