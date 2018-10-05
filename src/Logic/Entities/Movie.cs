namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Movie.
	/// </summary>
	public class Movie
	{
		/// <summary>
		/// Unique key from DB.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Movie name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Source file name of subtitles.
		/// </summary>
		public string SubtitlesFileName { get; set; }

		/// <summary>
		/// Customer (each movie is personal for customer).
		/// </summary>
		public int CustomerId { get; set; }

		/// <summary>
		/// Language Id ()each movie can be linked to 1 language only!)
		/// </summary>
		public int LanguageId { get; set; }
	}
}
