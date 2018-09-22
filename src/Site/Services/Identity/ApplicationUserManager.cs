using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Services.Identity
{
	public class ApplicationUserManager : UserManager<Customer>
	{
		public ApplicationUserManager(IUserStore<Customer> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<Customer> passwordHasher,
			IEnumerable<IUserValidator<Customer>> userValidators, IEnumerable<IPasswordValidator<Customer>> passwordValidators,
			ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<Customer>> logger)
			: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public override Task<IdentityResult> UpdateAsync(Customer user)
		{
			if (!string.IsNullOrEmpty(user.Password))
			{
				user.PasswordHash = PasswordHasher.HashPassword(user, user.Password);
			}

			return base.UpdateAsync(user);
		}

		/// <summary>
		/// Simple override to get password hasher here.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="token"></param>
		/// <param name="newPassword"></param>
		/// <returns></returns>
		public override async Task<IdentityResult> ResetPasswordAsync(Customer user, string token, string newPassword)
		{
			user.PasswordHash = PasswordHasher.HashPassword(user, newPassword);

			await UserManager.Instance.ChangePassword(user);

			return await Task.FromResult(new IdentityResult());
		}
	}
}
