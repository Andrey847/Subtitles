using Almagu.FileWatcher.Logic.Entities;
using System.Collections.Generic;

namespace SubtitlesLearn.Models
{
	/// <summary>
	/// Represents one word from the DB (personal for Customer).
	/// </summary>
	public class Word : IIdentifiable
    {
		/// <summary>
		/// Simple Id from DB.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Word value.
		/// </summary>
		public string English { get; set; }

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
