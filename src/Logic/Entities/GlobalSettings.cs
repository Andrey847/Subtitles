using SubtitlesLearn.Logic.Interfaces;
using System;

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

		/// <summary>
		/// Reference to UI method for getting complete server url.
		/// </summary>
		public Func<string, string> GetFullUrl { get; set; }
	}
}
