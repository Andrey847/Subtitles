﻿using SubtitlesLearn.Logic.Entities;
using System;
using System.Data;
using System.Threading.Tasks;
using static SubtitlesLearn.Logic.Dal.DbHelper;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// DAL access to all user(customer) functions.
	/// </summary>
	internal static class UserAccess
	{
		/// <summary>
		/// Updates customer's password.
		/// </summary>
		/// <param name="user"></param>
		internal static async Task UpdatePassword(Customer user)
		{
			await ExecuteNonQueryAsync("dbo.usp_Customer_Password_Update",
				(p) =>
				{
					p.Add(nameof(Customer.Email), SqlDbType.NVarChar).Value = user.Email;
					p.Add(nameof(Customer.PasswordHash), SqlDbType.NVarChar).Value = user.PasswordHash;
				});
		}

		/// <summary>
		/// Creates customer (requires email and password hash only. Other parameters are by the default).
		/// </summary>
		/// <param name="user"></param>
		internal static async Task CreateUser(Customer user)
		{
			await ExecuteNonQueryAsync("dbo.usp_Customer_Create",
				(p) =>
				{
					p.Add(nameof(Customer.Email), SqlDbType.NVarChar).Value = user.Email;
					p.Add(nameof(Customer.PasswordHash), SqlDbType.NVarChar).Value = user.PasswordHash;
				});
		}

		/// <summary>
		/// Creates customer (requires email and password hash only. Other parameters are by the default).
		/// </summary>
		/// <param name="user"></param>
		internal static async Task<Customer> GetUser(string email = null, int? customerId = null)
		{
			return await ExecuteProcedureAsync<Customer>("dbo.usp_Customer_Get",
				(p) =>
				{
					p.Add(nameof(Customer.Email), SqlDbType.NVarChar).Value = email;
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				},
				(m) =>
				{
					return new Customer()
					{
						Email = (string)m["Email"],
						PasswordHash = (string)m["PasswordHash"],
						Name = (string)m["Name"],
						IsBlocked = Convert.ToBoolean(m["IsBlocked"]),
						IsConfirmed = Convert.ToBoolean(m["IsConfirmed"]),
						Id = (int)m["CustomerId"],
						RestorePasswordCode = (string)m["RestorePasswordCode"]
					};
				});
		}

		/// <summary>
		/// Returns restore code.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		internal static async Task<string> GetRestoreCode(string email)
		{
			return await ExecuteScalarAsync<string>("dbo.usp_Customer_RestoreCode_Get",
														p => p.Add("Email", SqlDbType.NVarChar).Value = email);
		}

		/// <summary>
		/// Verifies restore password code.
		/// </summary>
		/// <param name="restorePasswordCode"></param>
		/// <returns></returns>
		internal static async Task<string> VerifyPasswordRestore(string restorePasswordCode)
		{
			return await ExecuteScalarAsync<string>("dbo.usp_Customer_RestoreCode_Verify",
														p => p.Add("RestorePasswordCode", SqlDbType.NVarChar).Value = restorePasswordCode);
		}
	}
}
