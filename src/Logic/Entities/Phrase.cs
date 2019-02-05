using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Initial phrase from subtitles.
	/// </summary>
	[DebuggerDisplay("{Value}")]
	public class Phrase
	{
		#region Constrants

		private const string TIMESPAN_FORMAT = @"hh\:mm\:ss\.fff";

		#endregion Constants

		#region Properties

		/// <summary>
		/// Phrase itself.
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Phrase order number in this movie.
		/// </summary>
		public int OrderNumber { get; set; }

		/// <summary>
		/// Time from.
		/// </summary>
		[XmlIgnore]
		public TimeSpan TimeFrom { get; set; }

		/// <summary>
		/// Field for xml serialization to save in MS SQL.
		/// </summary>
		public string TimeFromSql
		{
			get => TimeFrom.ToString(TIMESPAN_FORMAT, CultureInfo.InvariantCulture);
			set
			{
				// do nothing. For serialization only.
			}
		}

		/// <summary>
		/// Time to.
		/// </summary>
		[XmlIgnore]
		public TimeSpan TimeTo { get; set; }

		/// <summary>
		/// Field for xml serialization to save in MS SQL.
		/// </summary>
		public string TimeToSql
		{
			get => TimeTo.ToString(TIMESPAN_FORMAT, CultureInfo.InvariantCulture);
			set
			{
				// do nothing. For serialization only.
			}
		}

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
				result = another.Value.Equals(this.Value)
					&& another.TimeFrom == this.TimeFrom;
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

			try
			{

				TimeFrom = TimeSpan.ParseExact(m.Groups["from"].Value, format, CultureInfo.InvariantCulture);
				TimeTo = TimeSpan.ParseExact(m.Groups["to"].Value, format, CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Unable to parse {timing}", ex);
			}
		}

		/// <summary>
		/// Clones current phrase.
		/// </summary>
		/// <returns></returns>
		public Phrase Clone()
		{
			return (Phrase)this.MemberwiseClone();
		}

		#endregion Methods
	}
}
