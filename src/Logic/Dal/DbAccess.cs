using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using static SubtitlesLearn.Logic.Dal.DbHelper;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// Main DAL class.
	/// </summary>
	public static class DbAccess
	{
		#region Helper methods		

		/// <summary>
		/// Returns simple word from the DB. If it is not exists - adds to DB.
		/// </summary>
		/// <param name="english"></param>
		/// <returns></returns>
		public static Word GetWord(int customerId, Word word, string fileName)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Merge", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("CustomerId", System.Data.SqlDbType.Int).Value = customerId;
				procedure.Parameters.Add("English", System.Data.SqlDbType.NVarChar).Value = word.English;
				procedure.Parameters.Add("Frequency", System.Data.SqlDbType.Int).Value = word.Frequency;
				procedure.Parameters.Add("FileName", System.Data.SqlDbType.VarChar).Value = fileName;
				procedure.Parameters.Add("Phrases", System.Data.SqlDbType.Xml).Value = SerializationHelper.Serialize(word.Phrases);

				conn.Open();

				Word result = new Word();

				SqlDataReader reader = procedure.ExecuteReader();

				reader.Read();

				result.Id = Convert.ToInt32(reader["WordId"]);
				result.English = Convert.ToString(reader["English"]);
				result.IsKnown = Convert.ToBoolean(reader["IsKnown"]);
				result.Translation = Convert.ToString(reader["Translation"]);

				conn.Close();

				return result;
			}
		}

		/// <summary>
		/// Logs data to the DB.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <param name="now"></param>
		/// <param name="level"></param>
		internal static void Log(string message, string details, DateTime now, LogLevel level)
		{
			ExecuteNonQuery("dbo.usp_Log_Add", message, details, now, level);
		}

		/// <summary>
		/// Logs data to the DB.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="details"></param>
		/// <param name="now"></param>
		/// <param name="level"></param>
		internal static async Task LogAsync(string message, string details, DateTime now, LogLevel level)
		{
			await ExecuteNonQueryAsync("dbo.usp_Log_Add", (p) =>
			{
				p.Add("Message", SqlDbType.NVarChar).Value = message;
				p.Add("Details", SqlDbType.NVarChar).Value = details;
				p.Add("LogLevelId", SqlDbType.Int).Value = (int)level;
				p.Add("Now", SqlDbType.DateTime).Value = now;
			});
		}

		/// <summary>
		/// Returns sound for the word.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public static byte[] GetSound(int customerId, string word)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("dbo.usp_Word_Sound_Get", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("CustomerId", SqlDbType.Int).Value = customerId;
				procedure.Parameters.Add("English", SqlDbType.NVarChar).Value = word;

				conn.Open();

				byte[] result = procedure.ExecuteScalar() as byte[];

				conn.Close();

				return result;
			}
		}

		/// <summary>
		/// Adds sound to the word.
		/// </summary>
		/// <param name="word"></param>
		/// <param name="sound"></param>
		public static void AddSound(string word, byte[] sound)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Sound_Add", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("English", System.Data.SqlDbType.NVarChar).Value = word;
				procedure.Parameters.Add("Wav", System.Data.SqlDbType.Binary).Value = sound;

				conn.Open();

				procedure.ExecuteNonQuery();

				conn.Close();
			}
		}

		/// <summary>
		/// Returns simple word from the DB.
		/// </summary>
		/// <param name="english"></param>
		/// <returns></returns>
		public static List<Word> GetAllWords(int customerId)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_All_Get", conn);
				procedure.Parameters.Add("CustomerId", SqlDbType.NVarChar).Value = customerId;

				conn.Open();

				List<Word> result = new List<Word>();

				SqlDataReader reader = procedure.ExecuteReader();

				while (reader.Read())
				{
					Word word = new Word();
					word.Id = Convert.ToInt32(reader["WordId"]);
					word.English = Convert.ToString(reader["English"]);
					word.IsKnown = Convert.ToBoolean(reader["IsKnown"]);
					word.Translation = Convert.ToString(reader["Translation"]);
					word.Frequency = Convert.ToInt32(reader["Frequency"]);

					result.Add(word);
				}

				conn.Close();

				return result.OrderByDescending(item => item.Frequency).ToList();
			}
		}

		public static void SaveTranslation(Word word)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Translation_Save", conn);
				procedure.CommandType = CommandType.StoredProcedure;
				procedure.Parameters.Add("English", SqlDbType.NVarChar).Value = word.English;
				procedure.Parameters.Add("Translation", SqlDbType.NVarChar).Value = word.Translation;

				conn.Open();

				procedure.ExecuteNonQuery();

				conn.Close();
			}
		}

		public static void MarkLearned(Word word)
		{
			using (SqlConnection conn = new SqlConnection(DbHelper.ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Learned_Mark", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("English", System.Data.SqlDbType.NVarChar).Value = word.English;

				conn.Open();

				procedure.ExecuteNonQuery();

				conn.Close();
			}
		}

		#endregion Helper methods
	}
}
