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
		/// Executes stored procedure and returns single scalar value.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="createParameters"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		internal static async Task<T> ExecuteScalarAsync<T>(string procedureName, Action<SqlParameterCollection> createParameters)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand(procedureName, conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				createParameters(procedure.Parameters);

				await conn.OpenAsync();

				T result = (T)await procedure.ExecuteScalarAsync();

				conn.Close();

				return result;
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

		/// <summary>
		/// Executes stored procedure ASYNC and returns mapped object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="procedureName"></param>
		/// <param name="createParameters"></param>
		/// <param name="mapper"></param>
		/// <returns></returns>
		internal static async Task<T> ExecuteProcedureAsync<T>(string procedureName, Action<SqlParameterCollection> createParameters, Func<SqlDataReader, T> mapper)
			where T : class, new()
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand procedure = new SqlCommand(procedureName, conn);
				procedure.CommandType = System.Data.CommandType.StoredProcedure;
				createParameters(procedure.Parameters);

				await conn.OpenAsync();

				SqlDataReader reader = await procedure.ExecuteReaderAsync();

				await reader.ReadAsync();

				T result = mapper(reader);

				conn.Close();

				return result;
			}
		}

		/// <summary>
		/// Aux method to execute SQL query directly.
		/// It is used in unit test in most cases.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		internal static void ExecuteScalarQuery(string query)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand sql = new SqlCommand(query, conn);
				conn.Open();

				sql.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Aux method to execute SQL query directly.
		/// It is used in unit test in most cases.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		internal static T ExecuteScalarQuery<T>(string query)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString))
			{
				SqlCommand sql = new SqlCommand(query, conn);
				conn.Open();

				return (T)sql.ExecuteScalar();				
			}
		}
	}
}
