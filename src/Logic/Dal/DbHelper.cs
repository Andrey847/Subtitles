using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// Aux methods to work with DB.
	/// </summary>
	public static class DbHelper
	{
		#region Initialization part 

		/// <summary>
		/// Main application connection string.
		/// </summary>
		public static string ConnectionString { get; set; }

		#endregion Initialization part

		/// <summary>
		/// Executes stored procedure and returns mapped object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		internal static void ExecuteNonQuery(string procedureName, params object[] values)
		{
			ExecuteNonQuery(procedureName, (p) =>
			{
				foreach (object value in values)
				{
					p.Add(value);
				}
			});
		}

		/// <summary>
		/// Executes stored procedure and returns mapped object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="createParameters"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		internal static void ExecuteNonQuery(string procedureName, Action<SqlParameterCollection> createParameters)			
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand(procedureName, conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				createParameters(procedure.Parameters);

				conn.Open();

				procedure.ExecuteNonQuery();

				conn.Close();
			}
		}

		/// <summary>
		/// Executes stored procedure and returns mapped object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="createParameters"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		internal static async Task ExecuteNonQueryAsync(string procedureName, Action<SqlParameterCollection> createParameters)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand(procedureName, conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				createParameters(procedure.Parameters);

				await conn.OpenAsync();

				await procedure.ExecuteNonQueryAsync();

				conn.Close();
			}
		}

		/// <summary>
		/// Executes stored procedure and returns mapped object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="createParameters"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		internal static T ExecuteProcedure<T>(string procedureName, Action<SqlParameterCollection> createParameters, Func<SqlDataReader, T> mapper)
			where T : class, new()
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand(procedureName, conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				createParameters(procedure.Parameters);

				conn.Open();

				SqlDataReader reader = procedure.ExecuteReader();

				reader.Read();

				T result = mapper(reader);

				conn.Close();

				return result;
			}
		}
	}
}
