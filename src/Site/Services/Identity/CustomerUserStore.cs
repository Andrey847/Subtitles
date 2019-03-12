using Microsoft.AspNetCore.Identity;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Services.Identity
{
	/// <summary>
	/// Хранилище? :) для работы с пользователями.
	/// </summary>
	public class CustomerUserStore : IUserStore<Customer>,
			IUserPasswordStore<Customer>,
			IUserRoleStore<Customer>
	{
		#region IUserStore implementation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<IdentityResult> CreateAsync(Customer user, CancellationToken cancellationToken)
		{
			IdentityResult result;

			// check by guest id and login email separately
			if (await UserManager.Instance.GetUser(user.Email) == null)
			{
				await UserManager.Instance.CreateUser(user);
				result = IdentityResult.Success;
			}
			else
			{
				result = IdentityResult.Failed(new IdentityError() { Description = "User already exists." });
			}

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<IdentityResult> DeleteAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			// there is nothing to dispose
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Customer> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			return await UserManager.Instance.GetUser(int.Parse(userId));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="normalizedUserName"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<Customer> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			return await UserManager.Instance.GetUser(normalizedUserName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<string> GetNormalizedUserNameAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates a user (without password, just to get the Id).
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<string> GetUserIdAsync(Customer user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Id.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<string> GetUserNameAsync(Customer user, CancellationToken cancellationToken)
		{
			return Task.FromResult(user.Email);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="normalizedName"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task SetNormalizedUserNameAsync(Customer user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			// У Customer user нет NormalizedName - 
			return Task.FromResult<object>(null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="userName"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task SetUserNameAsync(Customer user, string userName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<IdentityResult> UpdateAsync(Customer user, CancellationToken cancellationToken)
		{
			await UserManager.Instance.CreateUser(user);
			return IdentityResult.Success;
		}

		#endregion IUserStore implementation

		#region IUserPasswordStore implementation

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<string> GetPasswordHashAsync(Customer user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(user.PasswordHash);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task<bool> HasPasswordAsync(Customer user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <param name="passwordHash"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public Task SetPasswordHashAsync(Customer user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;

			return Task.FromResult<object>(null);
		}

		#endregion IUserPasswordStore implementation

		#region IUserRoleStore implementation

		public Task AddToRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveFromRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<string>> GetRolesAsync(Customer user, CancellationToken cancellationToken)
		{
			return Task.FromResult<IList<string>>(new string[] { user.Role.ToString() }.ToList());
		}

		public Task<bool> IsInRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return Task.FromResult(true);
		}

		public Task<IList<Customer>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion IUserRoleStore implementation
	}
}
