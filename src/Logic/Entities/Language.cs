namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Language.
	/// </summary>
	public class Language
	{
		/// <summary>
		/// Language identity.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Unique language code. Used for user settings.
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// Language name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Name of image file with this language flag.
		/// </summary>
		public string BannerImage { get; set; }

		/// <summary>
		/// Code to be used to get speech from google.
		/// </summary>
		public string GoogleCode { get; set; }
	}
}
