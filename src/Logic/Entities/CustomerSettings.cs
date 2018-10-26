﻿namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Personal customer settings.
	/// </summary>
	public class CustomerSettings
	{
		/// <summary>
		/// Code of language that currently used by Customer.
		/// </summary>
		public string CurrentLanguageCode { get; set; }

		/// <summary>
		/// How many words (max) should be shown on the main page.
		/// </summary>
		public int UnknownWordMax { get; set; }

		/// <summary>
		/// Customer Id.
		/// </summary>
		public int CustomerId { get; set; }
	}
}
