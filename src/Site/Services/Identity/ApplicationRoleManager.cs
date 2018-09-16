using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SubtitlesLearn.Logic.Entities;
using System.Collections.Generic;

namespace SubtitlesLearn.Site.Services.Identity
{
	public class ApplicationRoleManager : RoleManager<CustomerRole>
	{
		public ApplicationRoleManager(IRoleStore<CustomerRole> store, IEnumerable<IRoleValidator<CustomerRole>> roleValidators, ILookupNormalizer keyNormalizer,
			IdentityErrorDescriber errors, ILogger<RoleManager<CustomerRole>> logger)
			: base(store, roleValidators, keyNormalizer, errors, logger)
		{
		}
	}
}
