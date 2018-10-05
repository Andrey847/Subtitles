using SubtitlesLearn.Logic.Entities;
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
	}
}
