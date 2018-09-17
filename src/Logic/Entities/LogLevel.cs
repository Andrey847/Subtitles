namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Log level.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Application fatal error (unprocessed at all).
		/// Application stops.
		/// </summary>
		FatalError = 1,

		/// <summary>
		/// Processed application error.
		/// Application works.
		/// </summary>
		Error = 2,

		/// <summary>
		/// Information message.
		/// </summary>
		Information = 3,

		/// <summary>
		/// Various debugging messages.
		/// </summary>
		Debug = 4
	}
}
