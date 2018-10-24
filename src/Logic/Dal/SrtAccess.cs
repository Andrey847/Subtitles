using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
using System;
using System.Data;
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
		internal static async Task<Phrase[]> GetPhrases(int wordId)
		{
			return (await ExecuteListAsync("dbo.usp_Phrase_Get",
				(p) =>
				{
					p.Add("WordId", SqlDbType.Int).Value =wordId;					
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
		internal static async Task<Movie[]> GetMovies(int customerId)
		{
			return (await ExecuteListAsync("dbo.usp_Movie_Get",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				},
				(m) =>
					new Movie()
					{
						Id = (int)m["MovieId"],
						Name = m["Name"] as string,
						SubtitlesFileName = m["SubtitlesFileName"] as string,
						CustomerId = (int)m["CustomerId"],
						LanguageId = (int)m["LanguageId"]
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
		internal static async Task<bool> ImportWord(Word word, string fileName)
		{
			bool isAdded = await ExecuteScalarAsync<bool>("dbo.usp_Word_Merge",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = word.CustomerId;
					p.Add("Source", SqlDbType.NVarChar).Value = word.Source;
					p.Add("FileName", SqlDbType.VarChar).Value = fileName;
					p.Add("Phrases", SqlDbType.Xml).Value = SerializationHelper.Serialize(word.Phrases);
				});

			return isAdded;
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
	}
}
