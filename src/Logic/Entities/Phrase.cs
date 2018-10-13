using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Initial phrase from subtitles.
	/// </summary>
	public class Phrase
	{
		#region Properties

		/// <summary>
		/// Phrase itself.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Time from.
		/// </summary>
		public TimeSpan? TimeFrom { get; set; }

		/// <summary>
		/// Time to.
		/// </summary>
		public TimeSpan? TimeTo { get; set; }

		#endregion Properties

		#region Construction

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

		#endregion Construction

		#region Methods

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

		/// <summary>
		/// Sets timing for this phrase.
		/// </summary>
		/// <param name="timing"></param>
		public void SetTiming(string timing)
		{
			Match m = Regex.Match(timing, @"(?<from>[\d:,]*)\s-->\s(?<to>[\d:,]*)");

			if (!m.Success)
			{
				throw new InvalidOperationException($"Unable to define timing from phrase {timing}");
			}

			const string format = @"hh\:mm\:ss\,fff";

			TimeFrom = TimeSpan.ParseExact(m.Groups["from"].Value, format, CultureInfo.InvariantCulture);
			TimeTo = TimeSpan.ParseExact(m.Groups["to"].Value, format, CultureInfo.InvariantCulture);
		}

		#endregion Methods
	}
}
