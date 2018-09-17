using Moq;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Logic.Tests
{
	public class UserManagerTest
	{
		public UserManagerTest()
		{
			//	Unfortunately, standard invoke doesn't work. System.Configuration.ConfigurationManager.ConnectionStrings["Main"].ConnectionString;
			// So do workaround.
			Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			ConfigurationFileMap fileMap = new ConfigurationFileMap($"{currentAssembly.ManifestModule.ToString()}.config"); //Path to your config file
			Configuration configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);			
			DbHelper.ConnectionString = configuration.ConnectionStrings.ConnectionStrings["Main"].ConnectionString;

			UserManager.Instance.Log = new Mock<ILogging>(MockBehavior.Loose).Object;
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
			}
			finally
			{
				DbHelper.ExecuteScalarQuery($@"DELETE FROM Customer WHERE Email = '{customer.Email}'");
			}
		}
	}
}
