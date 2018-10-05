using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Processes srt file to produce words and other stuff.
	/// </summary>
	public class SrtManager : ManagerBase
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
					string number = reader.ReadLine(); // number
					reader.ReadLine(); // timing

					string sentence = string.Empty;
					string currentLine;

					while (!string.IsNullOrWhiteSpace(currentLine = reader.ReadLine()))
					{
						currentLine = currentLine.Trim();

						// it is dialog case. split each line to separate sentence.
						if (currentLine.StartsWith("-") && !string.IsNullOrEmpty(sentence))
						{
							result.AddRange(SplitSentance(sentence));
						}

						// it is ok as in most cases subtitles contains 2 rows max int the block.
						sentence += " " + currentLine;
					}

					if (!string.IsNullOrEmpty(sentence))
					{
						result.AddRange(SplitSentance(sentence));
					}

					if (currentLine == null)
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
			sentance = sentance.Trim();
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

		/// <summary>
		/// Returns all phrases for required word.
		/// </summary>
		/// <param name="wordId"></param>
		/// <returns></returns>
		public async Task<Phrase[]> GetPhrases(int wordId)
		{
			return await SrtAccess.GetPhrases(wordId);
		}

		/// <summary>
		/// Returns movies for the customer.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public async Task<Movie[]> GetMovies(int customerId)
		{
			return await SrtAccess.GetMovies(customerId);
		}

		/// <summary>
		/// Imports words to the database.
		/// </summary>
		/// <param name="srtFile">File from browser.</param>
		/// <returns></returns>
		public async Task<ImportReponse> ImportWords(int customerId, MemoryStream srtFile, string fileName)
		{
			if (srtFile == null)
			{
				throw new ArgumentNullException(nameof(srtFile));
			}

			ImportReponse response = new ImportReponse();
			string srt = Encoding.UTF8.GetString(srtFile.ToArray());
			Word[] words = GetWords(srt);

			// check each word with DB.
			foreach (Word word in words)
			{
				word.CustomerId = customerId;
				bool isAdded = await SrtAccess.ImportWord(word, fileName);

				if (isAdded)
				{
					response.NewWords++;
				}
				response.TotalWords++;
			}

			return response;
		}

		#endregion Methods
	}
}
