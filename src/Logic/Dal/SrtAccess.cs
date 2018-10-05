using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
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
		/// Imports (if it is required) word to DB.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		internal static async Task ImportWord(Word word, string fileName)
		{
			await ExecuteNonQueryAsync("dbo.usp_Word_Merge",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = word.CustomerId;
					p.Add("English", SqlDbType.NVarChar).Value = word.English;
					p.Add("Frequency", SqlDbType.Int).Value = word.Frequency;
					p.Add("FileName", SqlDbType.VarChar).Value = fileName;
					p.Add("Phrases", SqlDbType.Xml).Value = SerializationHelper.Serialize(word.Phrases);
				});
		}
	}
}
