﻿using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SubtitlesLearn.Logic.Manager
{
	/// <summary>
	/// Processes srt file to produce words and other stuff.
	/// </summary>
	public class SrtManager
	{
		#region Singleton

		private static Lazy<SrtManager> _instance = new Lazy<SrtManager>(() => new SrtManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static SrtManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private SrtManager()
		{

		}

		#endregion Singleton

		#region Methods

		/// <summary>
		/// Returns all words in the srt file.
		/// </summary>
		/// <param name="srtContent"></param>
		/// <returns></returns>
		public Word[] GetWords(string srtContent)
		{
			List<Word> result = new List<Word>();

			using (StringReader reader = new StringReader(srtContent))
			{
				while (true)
				{
					reader.ReadLine(); // number
					reader.ReadLine(); // timing

					string sentance;

					while (!string.IsNullOrEmpty(sentance = reader.ReadLine()))
					{
						result.AddRange(SplitSentance(sentance));
					}

					if (sentance == null)
					{
						// end of file
						break;
					}
				}
			}

			Word[] words = result.GroupBy(item => item.English)
							.Select(item => new Word()
							{
								English = item.Key,
								Frequency = item.Count(),
								Phrases = new List<Phrase>(item.SelectMany(jtem => jtem.Phrases))
							})
							.ToArray();

			return words;
		}

		private Word[] SplitSentance(string sentance)
		{
			sentance = sentance.Replace(".", string.Empty)
				.Replace("?", string.Empty)
				.Replace("!", string.Empty)
				.Replace("'s", string.Empty)
				.Replace("\"", string.Empty)
				.Replace(",", string.Empty)
				.Replace(":", string.Empty)
				.Replace("[", string.Empty)
				.Replace("[", string.Empty)
				.Replace("(", string.Empty)
				.Replace(")", string.Empty);

			string[] words = sentance.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			List<Word> result = new List<Word>();

			Phrase phrase = new Phrase()
			{
				Value = sentance
			};

			foreach (string word in words)
			{
				if (word.Length > 2 && !word.Contains("<") && !word.Contains(">") && !word.Contains("'"))
				{
					result.Add(new Word() { English = word });
				}
			}

			result.ForEach(item => item.Phrases.Add(phrase));

			return result.ToArray();
		}

		#endregion Methods
	}
}
