﻿using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Processes srt file to produce words and other stuff.
	/// </summary>
	public class SrtManager : ManagerBase
	{
		#region Events

		/// <summary>
		/// Raised if srt upload progress changed.
		/// </summary>
		public event EventHandler<SrtProgressArgs> SrtUploadProgress;

		private void OnSrtUploadProgress(int customerId, int percentCompleted)
		{
			SrtUploadProgress?.Invoke(this, new SrtProgressArgs(customerId, percentCompleted));
		}

		#endregion Events

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
		/// Sets learned/unlearned state for the word.
		/// </summary>
		public async Task SetLearned(Word word, bool isLearned)
		{
			await Log.LogDebug("Set learned state", $"CustomerId: {word.CustomerId}, word: {word.Source}, is learned: {isLearned}");
			await SrtAccess.SetLearned(word, isLearned);
		}

		/// <summary>
		/// Returns all words in the srt file.
		/// </summary>
		/// <param name="srtContent"></param>
		/// <returns></returns>
		public UploadPhrase[] GetUploadPhrases(string srtContent)
		{
			List<UploadPhrase> result = new List<UploadPhrase>();
			int orderNumber = 0;

			using (StringReader reader = new StringReader(srtContent))
			{
				while (true)
				{
					orderNumber++;

					UploadPhrase phrase = new UploadPhrase();

					string number = reader.ReadLine(); // number
					if (number == null)
					{
						// end of text.
						break;
					}

					phrase.SetTiming(reader.ReadLine()); // timing
					phrase.OrderNumber = orderNumber;

					string currentLine;

					while (!string.IsNullOrWhiteSpace(currentLine = reader.ReadLine()))
					{
						currentLine = currentLine.Trim();

						// it is dialog case. split each line to separate sentence.
						if (currentLine.StartsWith("-") && !string.IsNullOrEmpty(phrase.Value))
						{
							phrase.Words.AddRange(SplitSentence(phrase));
							result.Add(phrase);

							// To start new phrase							
							TimeSpan from = phrase.TimeFrom;
							TimeSpan to = phrase.TimeTo;

							phrase = new UploadPhrase();

							orderNumber++;
							phrase.OrderNumber = orderNumber;
							phrase.Value = currentLine;
						}
						else
						{
							// it is ok as in most cases subtitles contains 2 rows max int the block.
							phrase.Value += " " + currentLine;
						}
					}

					if (!string.IsNullOrEmpty(phrase.Value))
					{
						phrase.Words.AddRange(SplitSentence(phrase));
						result.Add(phrase);
					}

					if (currentLine == null)
					{
						// end of file
						break;
					}
				}
			}

			// We do not need phrases without words
			result.RemoveAll(item => item.Words.Count == 0);

			return result.ToArray();
		}

		/// <summary>
		/// Splits sentence by words.
		/// </summary>
		/// <param name="phrase"></param>
		/// <returns></returns>
		internal string[] SplitSentence(Phrase phrase)
		{
			string sentence = phrase.Value;

			// remove text in [] 
			sentence = Regex.Replace(sentence, @"\[.*?\]", string.Empty);

			// Remove ALLCAPITALS words. usually the means Aux words like actor name etc.
			sentence = Regex.Replace(sentence, @"[A-Z]{2,100}:\s", string.Empty);

			phrase.Value = sentence.Trim();

			const string space = " ";

			sentence = sentence
				.Replace(".", space)
				.Replace("?", space)
				.Replace("!", space)
				.Replace("'s", space)
				.Replace("\"", space)
				.Replace(",", space)
				.Replace(":", space)
				.Replace("[", space)
				.Replace("]", space)
				.Replace("(", space)
				.Replace(")", space)
				.Replace(";", space);

			string[] words = sentence.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			List<string> result = new List<string>();

			foreach (string word in words)
			{
				if (word.Length > 2 && !word.Contains("<") && !word.Contains(">") && !word.Contains("'"))
				{
					result.Add(word);
				}
			}		

			return result.ToArray();
		}

		/// <summary>
		/// Returns all phrases for required word.
		/// </summary>
		/// <param name="wordId"></param>
		/// <returns></returns>
		public async Task<Phrase[]> GetPhrases(int wordId, int? movieId)
		{
			return await SrtAccess.GetPhrases(wordId, movieId);
		}

		/// <summary>
		/// Returns movies for the customer.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		public async Task<Movie[]> GetMovies(int customerId, bool showArchived, string languageCode = "eng")
		{
			return await SrtAccess.GetMovies(customerId, showArchived, languageCode);
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
			UploadPhrase[] phrases = GetUploadPhrases(srt);

			int currentPercentage = -1;

			// Upload each phrase to DB.
			for (int i = 0; i < phrases.Length; i++)
			{
				UploadPhrase phrase = phrases[i];

				phrase.CustomerId = customerId;
				int newWords = await SrtAccess.ImportPhrase(phrase, fileName);

				response.NewWords += newWords;				

				int percentage = (int)((i / (float)phrases.Length) * 100);

				if (percentage != currentPercentage)
				{
					currentPercentage = percentage;
					OnSrtUploadProgress(customerId, currentPercentage);
				}
			}

			OnSrtUploadProgress(customerId, 100);

			return response;
		}		

		/// <summary>
		/// Removes movie.
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		public async Task DeleteMovie(int customerId, int movieId)
		{
			await Log.LogInfo("Deleting movie", $"CustomerId: {customerId}, MovieId: {movieId}");
			await SrtAccess.DeleteMovie(customerId, movieId);
		}

		/// <summary>
		/// Returns all languages that system supports.
		/// </summary>
		/// <returns></returns>
		public async Task<Language[]> GetLanguages()
		{
			return await SrtAccess.GetLanguages();
		}

		/// <summary>
		/// Renames movie.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="movieId"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		public async Task<string> RenameMovie(int customerId, int movieId, string newName)
		{
			await Log.LogInfo("Renaming movie", $"CustomerId: {customerId}, MovieId: {movieId}, NewName: {newName}");

			return await SrtAccess.RenameMovie(customerId, movieId, newName);
		}

		/// <summary>
		/// Sets or unsets archived state for the movie.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="movieId"></param>
		/// <param name="archive"></param>
		/// <returns></returns>
		public async Task SetArchiveState(int customerId, int movieId, bool archive)
		{
			await Log.LogInfo("Set movie archive state", $"CustomerId: {customerId}, MovieId: {movieId}, state: {archive}");

			await SrtAccess.SetArchiveState(customerId, movieId, archive);
		}

		/// <summary>
		/// Returns top unlearned words.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="movieId"></param>
		/// <returns></returns>
		public async Task<List<Word>> GetAllWords(int customerId, int? movieId = null)
		{
			return await SrtAccess.GetAllWords(customerId, movieId);
		}

		#endregion Methods
	}
}
