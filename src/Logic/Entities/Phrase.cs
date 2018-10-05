namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Initial phrase from subtitles.
	/// </summary>
	public class Phrase
	{
		/// <summary>
		/// Phrase itself.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// For serialization.
		/// </summary>
		public Phrase()
		{ }

		/// <summary>
		/// For fast use.
		/// </summary>
		/// <param name="value"></param>
		public Phrase(string value)
		{
			Value = value;
		}

		/// <summary>
		/// <see cref="Equals(object)"/>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool result = false;
			Phrase another = obj as Phrase;

			if (obj != null)
			{
				result = obj.Equals(this);
			}

			return result;
		}

		/// <summary>
		/// <see cref="GetHashCode"/>
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
