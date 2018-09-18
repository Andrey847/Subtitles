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
    }
}
