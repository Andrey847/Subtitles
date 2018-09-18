using SubtitlesLearn.Logic.Interfaces;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Global application settings.
	/// </summary>
	public class GlobalSettings : IGlobalSettings
	{
		/// <summary>
		/// Base application path.
		/// </summary>
		public string BasePath { get; set; }
	}
}
