namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Response of srt file import.
	/// </summary>
	public class ImportReponse
    {
		/// <summary>
		/// Total word count in the file.
		/// </summary>
		public int TotalWords { get; set; }

		/// <summary>
		/// New words (not in the customr's dictionary).
		/// </summary>
		public int NewWords { get; set; }
    }
}
