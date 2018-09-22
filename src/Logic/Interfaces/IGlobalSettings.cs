using System;

namespace SubtitlesLearn.Logic.Interfaces
{
	/// <summary>
	/// Represents global application settings.
	/// </summary>
	public interface IGlobalSettings
    {
		/// <summary>
		/// Base (root) application path.
		/// </summary>
		string BasePath { get; set; }

		/// <summary>
		/// Reference to UI method for getting complete server url.
		/// </summary>
		Func<string, string> GetFullUrl { get; set; }
	}
}
