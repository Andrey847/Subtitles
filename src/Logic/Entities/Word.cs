using SubtitlesLearn.Logic.Dal;
using System.Collections.Generic;
using System.Diagnostics;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Represents one word from the DB (personal for Customer).
	/// </summary>
	[DebuggerDisplay("{Source}")]
	public class Word : IIdentifiable
    {
		/// <summary>
		/// Simple Id from DB.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Customer identity.
		/// </summary>
		public int CustomerId { get; set; }

		/// <summary>
		/// Word value.
		/// </summary>
		public string Source { get; set; }

		/// <summary>
		/// Translation of the word.
		/// </summary>
		public string Translation { get; set; }

		/// <summary>
		/// Is word known by user?
		/// </summary>
		public bool IsKnown { get; set; }

		/// <summary>
		/// Count of occurance of this word.
		/// </summary>
		public int Frequency { get; set; }

		/// <summary>
		/// Phrases which contain this word.
		/// </summary>
		public List<Phrase> Phrases { get; set; } = new List<Phrase>();		
	}
}
