using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Manager;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// Main DAL class.
	/// </summary>
	public static class DbAccess
	{
		#region Initialization part 

		/// <summary>
		/// Main application connection string.
		/// </summary>
		public static string ConnectionString { get; set; }

		#endregion Initialization part

		#region Helper methods

		/// <summary>
		/// Returns simple word from the DB.
		/// </summary>
		/// <param name="english"></param>
		/// <returns></returns>
		public static Word GetWord(Word word, string fileName)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Get", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
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
		/// Returns sound for the word.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public static byte[] GetSound(string word)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Sound_Get", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("English", System.Data.SqlDbType.NVarChar).Value = word;

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
			using (SqlConnection conn = new SqlConnection(ConnectionString))
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
		public static List<Word> GetAllWords()
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_All_Get", conn);

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
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand("usp_Word_Translation_Save", conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				procedure.Parameters.Add("English", System.Data.SqlDbType.NVarChar).Value = word.English;
				procedure.Parameters.Add("Translation", System.Data.SqlDbType.NVarChar).Value = word.Translation;

				conn.Open();

				procedure.ExecuteNonQuery();

				conn.Close();
			}
		}

		public static void MarkLearned(Word word)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
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
