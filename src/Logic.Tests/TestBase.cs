using Moq;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Interfaces;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace SubtitlesLearn.Logic.Tests
{
	/// <summary>
	/// Class with base operations for all tests.
	/// </summary>
	public abstract class TestBase
	{
		protected string RootPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ;

		/// <summary>
		/// Common constructor.
		/// </summary>
		protected TestBase()
		{
			//	Unfortunately, standard invoke doesn't work. System.Configuration.ConfigurationManager.ConnectionStrings["Main"].ConnectionString;
			// So do workaround.
			Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			ConfigurationFileMap fileMap = new ConfigurationFileMap($"{currentAssembly.ManifestModule.ToString()}.config"); //Path to your config file
			Configuration configuration = ConfigurationManager.OpenMappedMachineConfiguration(fileMap);
			DbHelper.ConnectionString = configuration.ConnectionStrings.ConnectionStrings["Main"].ConnectionString;

			// Mocking of some aux services.
			UserManager.Instance.Log = new Mock<ILogging>(MockBehavior.Loose).Object;
			UserManager.Instance.EmailNotifier = new Mock<IEmailNotifier>().Object;

			Mock<IGlobalSettings> settingMock = new Mock<IGlobalSettings>();
			settingMock.Setup(t => t.GetFullUrl).Returns(r => r);
			UserManager.Instance.GlobalSettings = settingMock.Object;
			
		}
	}
}
