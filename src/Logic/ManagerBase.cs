using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Interfaces;
using System.IO;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Base class for the managers with predefined common properties.
	/// </summary>
	public abstract class ManagerBase
	{
		/// <summary>
		/// Logging. 
		/// </summary>
		public ILogging Log { get; set; }

		/// <summary>
		/// Email notifications.
		/// </summary>
		public IEmailNotifier EmailNotifier {get; set;}

		/// <summary>
		/// Global application settings.
		/// </summary>
		public IGlobalSettings GlobalSettings { get; set; }

		/// <summary>
		/// Gets contant of the files from root of service path.
		/// </summary>
		/// <param name="relativePath"></param>
		/// <returns></returns>
		public string ReadFile(string relativePath)
		{
			return File.ReadAllText(Path.Combine(GlobalSettings.BasePath, relativePath));
		}
	}
}
