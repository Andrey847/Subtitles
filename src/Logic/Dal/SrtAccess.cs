using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static SubtitlesLearn.Logic.Dal.DbHelper;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// Access for all main subtitles methods (words, phrases, everything).
	/// </summary>
	internal static class SrtAccess
	{
		/// <summary>
		/// Updates customer's password.
		/// </summary>
		/// <param name="user"></param>
		internal static async Task<Phrase[]> GetPhrases(int wordId, int? movieId)
		{
			return (await ExecuteListAsync("dbo.usp_Phrase_Get",
				(p) =>
				{
					p.Add("WordId", SqlDbType.Int).Value = wordId;
					p.Add("MovieId", SqlDbType.Int).Value = movieId;
				},
				(m) =>
					new Phrase(m["Value"] as string)
				)).ToArray();
		}

		/// <summary>
		/// Returns movies for the customer.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		internal static async Task<Movie[]> GetMovies(int customerId, bool showArchivedMovies, string languageCode)
		{
			return (await ExecuteListAsync("dbo.usp_Movie_Get",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
					p.Add("ShowArchivedMovies", SqlDbType.Bit).Value = showArchivedMovies;
					p.Add("LanguageCode", SqlDbType.NVarChar).Value = languageCode;
				},
				(m) =>
					new Movie()
					{
						Id = (int)m["MovieId"],
						Name = m["Name"] as string,
						SubtitlesFileName = m["SubtitlesFileName"] as string,
						CustomerId = (int)m["CustomerId"],
						LanguageId = (int)m["LanguageId"],
						IsArchived = Convert.ToBoolean(m["IsArchived"])
					}
				)).ToArray();
		}

		/// <summary>
		/// Removes movie from DB.
		/// </summary>
		/// <param name="movieId"></param>
		/// <returns></returns>
		internal static async Task DeleteMovie(int customerId, int movieId)
		{
			await ExecuteNonQueryAsync("dbo.usp_Movie_Delete",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
					p.Add("MovieId", SqlDbType.Int).Value = movieId;
				});
		}

		/// <summary>
		/// Imports (if it is required) word to DB.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="fileName"></param>
		/// <returns>True if word is new and added to the dictionary.</returns>
		internal static async Task<int> ImportPhrase(UploadPhrase phrase, string fileName)
		{
			int newWords = await ExecuteScalarAsync<int>("dbo.usp_Phrase_Merge",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = phrase.CustomerId;
					p.Add("FileName", SqlDbType.VarChar).Value = fileName;
					p.Add("Value", SqlDbType.NVarChar).Value = phrase.Value;
					p.Add("TimeFrom", SqlDbType.Time).Value = phrase.TimeFrom;
					p.Add("TimeTo", SqlDbType.Time).Value = phrase.TimeTo;
					p.Add("OrderNumber", SqlDbType.Int).Value = phrase.OrderNumber;

					p.Add("Words", SqlDbType.Xml).Value = SerializationHelper.Serialize(phrase.Words);
				});

			return newWords;
		}

		/// <summary>
		/// Returns all possible langauges in the system.
		/// </summary>
		/// <returns></returns>
		internal static async Task<Language[]> GetLanguages()
		{
			return (await ExecuteListAsync<Language>("dbo.usp_Language_Get",
				(p) => { }, // no parameters
				(m) =>
				{
					return new Language()
					{
						Id = (int)m["LanguageId"],
						Code = (string)m["Code"],
						Name = (string)m["Name"],
						BannerImage = m["BannerImage"] as string,
						GoogleCode = m["GoogleCode"] as string
					};
				})).ToArray();
		}

		/// <summary>
		/// Renames movie.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="movieId"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		internal static async Task<string> RenameMovie(int customerId, int movieId, string newName)
		{
			return (await ExecuteScalarAsync<string>("dbo.usp_Movie_Rename", (p) =>
			{
				p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				p.Add("MovieId", SqlDbType.Int).Value = movieId;
				p.Add("NewName", SqlDbType.NVarChar).Value = newName;
			}));
		}

		/// <summary>
		/// Sets (or unsets) learned state for the word.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="isLearned"></param>
		/// <returns></returns>
		internal static async Task SetLearned(Word word, bool isLearned)
		{
			await ExecuteNonQueryAsync("dbo.usp_Word_Learned_Set", (p) =>
			{
				p.Add("CustomerId", SqlDbType.Int).Value = word.CustomerId;
				p.Add("Source", SqlDbType.NVarChar).Value = word.Source;
				p.Add("IsKnown", SqlDbType.Bit).Value = isLearned;
			});
		}

		/// <summary>
		/// Returns top unlearned words.
		/// </summary>
		/// <param name="customerId">Customer</param>
		/// <param name="movieId">if null all words of this customer is returned. If selected then returned words for this movie only.</param>
		/// <returns></returns>
		internal static async Task<List<Word>> GetAllWords(int customerId, int? movieId = null)
		{
			List<Word> result = await ExecuteListAsync<Word>("dbo.usp_Word_All_Get",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
					p.Add("MovieId", SqlDbType.Int).Value = movieId;
				},
				(m) =>
					new Word()
					{
						Id = Convert.ToInt32(m["WordId"]),
						CustomerId = Convert.ToInt32(m["CustomerId"]),
						Source = Convert.ToString(m["Source"]),
						IsKnown = Convert.ToBoolean(m["IsKnown"]),
						Translation = Convert.ToString(m["Translation"]),
						Frequency = Convert.ToInt32(m["Frequency"])
					}
				);

			return result.OrderByDescending(item => item.Frequency).ToList();			
		}

		/// <summary>
		/// Sets archive state for the movie.
		/// </summary>
		/// <param name="customerId"></param>
		/// <param name="movieId"></param>
		/// <param name="archive"></param>
		/// <returns></returns>
		internal static async Task SetArchiveState(int customerId, int movieId, bool archive)
		{
			await ExecuteNonQueryAsync("dbo.usp_Movie_Archive_Set", (p) =>
			{
				p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				p.Add("MovieId", SqlDbType.Int).Value = movieId;
				p.Add("IsArchived", SqlDbType.Bit).Value = archive;
			});
		}
	}
}
