using System.Collections.Generic;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Aux class for upload phrase and words.
	/// </summary>
	public class UploadPhrase : Phrase
	{
		/// <summary>
		/// List of words to be loaded.
		/// </summary>
		public List<string> Words { get; set; } = new List<string>();

		/// <summary>
		/// Customer.
		/// </summary>
		public int CustomerId { get; set; }
	}
}
