namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Response of srt file import.
	/// </summary>
	public class ImportReponse
    {
		/// <summary>
		/// New words (not in the customr's dictionary).
		/// </summary>
		public int NewWords { get; set; }
    }
}
