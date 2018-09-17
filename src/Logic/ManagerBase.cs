using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Interfaces;

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
	}
}
