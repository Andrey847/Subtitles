using Microsoft.AspNetCore.Identity;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SubtitlesLearn.Site.Services.Identity
{
	/// <summary>
	/// Roles store.
	/// </summary>
	public class RoleStore : IQueryableRoleStore<CustomerRole>, IUserRoleStore<Customer>
	{
		IQueryable<CustomerRole> IQueryableRoleStore<CustomerRole>.Roles => throw new NotImplementedException();

		public Task AddToRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> CreateAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> DeleteAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<Customer> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<Customer> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedUserNameAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<string>> GetRolesAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetUserIdAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetUserNameAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<Customer>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsInRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveFromRoleAsync(Customer user, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetNormalizedUserNameAsync(Customer user, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetUserNameAsync(Customer user, string userName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> UpdateAsync(Customer user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityResult> IRoleStore<CustomerRole>.CreateAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityResult> IRoleStore<CustomerRole>.DeleteAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		void IDisposable.Dispose()
		{
			// nothing do here
		}

		Task<CustomerRole> IRoleStore<CustomerRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<CustomerRole> IRoleStore<CustomerRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<CustomerRole>.GetNormalizedRoleNameAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<CustomerRole>.GetRoleIdAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<string> IRoleStore<CustomerRole>.GetRoleNameAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task IRoleStore<CustomerRole>.SetNormalizedRoleNameAsync(CustomerRole role, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task IRoleStore<CustomerRole>.SetRoleNameAsync(CustomerRole role, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityResult> IRoleStore<CustomerRole>.UpdateAsync(CustomerRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
