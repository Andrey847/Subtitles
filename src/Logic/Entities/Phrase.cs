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

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}
	}
}
