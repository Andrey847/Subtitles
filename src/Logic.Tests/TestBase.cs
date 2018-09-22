using Moq;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

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

			UserManager.Instance.Log = new Mock<ILogging>(MockBehavior.Loose).Object;
		}
	}
}
