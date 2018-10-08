using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Tests;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Logic.Tests
{
	/// <summary>
	/// Tests for user management.
	/// </summary>
	public class UserManagerTest : TestBase
	{
		/// <summary>
		/// Default constructor (can be used for initialization).
		/// </summary>
		public UserManagerTest()
		{			
		}

		[Fact]
		public async Task Validation_Creation_Email()
		{
			Customer customer = new Customer();
			customer.PasswordHash = "asdf";

			await Assert.ThrowsAsync<ArgumentException>(async () => await UserManager.Instance.CreateUser(customer));
		}

		[Fact]
		public async Task Validation_Creation_Password()
		{
			Customer customer = new Customer();
			customer.Email = "asdf@mail.ru";

			await Assert.ThrowsAsync<ArgumentException>(async () => await UserManager.Instance.CreateUser(customer));
		}

		/// <summary>
		/// Creation - getting user workflow.
		/// </summary>
		[Fact]
		public async Task UserWorkflow()
		{
			Customer customer = new Customer();
			customer.Email = Guid.NewGuid().ToString() + "@mail.ru";
			customer.PasswordHash = "asdf";

			try
			{
				await UserManager.Instance.CreateUser(customer);

				customer = await UserManager.Instance.GetUser(customer.Email);
				Assert.True(customer != null);
				Assert.True(string.IsNullOrEmpty(customer.Password));

				customer = await UserManager.Instance.GetUser(customer.Id);
				Assert.True(customer != null);
				Assert.True(string.IsNullOrEmpty(customer.Password));

				customer.PasswordHash = "123123123123";
				await UserManager.Instance.ChangePassword(customer);

				Assert.True(await UserManager.Instance.RestorePassword(new RestorePasswordRequest(customer.Email)));		
			}
			finally
			{
				DbHelper.ExecuteScalarQuery($@"DELETE FROM Customer WHERE Email = '{customer.Email}'");
			}
		}

		[Fact]
		public async void GetSettings()
		{
			Customer customer = await UserManager.Instance.GetUser("ag_a@mail.ru");

			Assert.NotNull(UserManager.Instance.GetSettings(customer.Id));
		}

		[Fact]
		public async void SaveSettings()
		{
			Customer customer = await UserManager.Instance.GetUser("ag_a@mail.ru");

			CustomerSettings settings = await UserManager.Instance.GetSettings(customer.Id);

			string currentCode = settings.CurrentLanguageCode;
			string newCode = currentCode + "$";

			settings.CurrentLanguageCode = newCode;

			await UserManager.Instance.UpdateSettings(settings);

			// Verify that updated
			settings = await UserManager.Instance.GetSettings(customer.Id);
			Assert.Equal(newCode, settings.CurrentLanguageCode);

			// return everything back
			settings.CurrentLanguageCode = currentCode;
			await UserManager.Instance.UpdateSettings(settings);
		}
	}
}
