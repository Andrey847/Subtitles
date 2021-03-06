﻿using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using SubtitlesLearn.Logic.Infrastructure.GoogleAuth;
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
					p.Add(nameof(Customer.PasswordHash), SqlDbType.NVarChar).Value = string.IsNullOrEmpty(user.PasswordHash) ? DBNull.Value : (object)user.PasswordHash;
					p.Add(nameof(Customer.ConfirmationCode), SqlDbType.NVarChar).Value = string.IsNullOrEmpty(user.ConfirmationCode) ? DBNull.Value : (object)user.ConfirmationCode;
					p.Add(nameof(Customer.IsConfirmed), SqlDbType.Bit).Value = user.IsConfirmed;
				});
		}

		/// <summary>
		/// Creates customer (requires email and password hash only. Other parameters are by the default).
		/// </summary>
		/// <param name="user"></param>
		internal static async Task<Customer> GetUser(string email = null, int? customerId = null)
		{
			return await ExecuteObjectAsync<Customer>("dbo.usp_Customer_Get",
				(p) =>
				{
					p.Add(nameof(Customer.Email), SqlDbType.NVarChar).Value = email;
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				},
				(m) =>
				{
					Customer c = new Customer()
					{
						Email = (string)m["Email"],
						PasswordHash = m["PasswordHash"] as string,
						Name = (string)m["Name"],
						IsBlocked = Convert.ToBoolean(m["IsBlocked"]),
						IsConfirmed = Convert.ToBoolean(m["IsConfirmed"]),
						Id = (int)m["CustomerId"],
						RestorePasswordCode = m["RestorePasswordCode"] as string
					};

					c.Role.Id = (int)m["CustomerRoleId"];

					return c;
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

		/// <summary>
		/// Returns settings for the customer.
		/// </summary>
		/// <param name="customerId"></param>
		/// <returns></returns>
		internal static async Task<CustomerSettings> GetSettings(int customerId)
		{
			return await ExecuteObjectAsync<CustomerSettings>("dbo.usp_Customer_Setting_Get",
				p => p.Add("CustomerId", SqlDbType.Int).Value = customerId,
				(m) =>
				{
					return new CustomerSettings()
					{
						CustomerId = customerId,
						CurrentLanguageCode = m["CurrentLanguageCode"] as string,
						UnknownWordMax = int.Parse(m["UnknownWordMax"] as string),
						ShowArchivedMovies = Convert.ToBoolean(int.Parse(m["ShowArchivedMovies"] as string))
					};
				}
				);
		}

		/// <summary>
		/// Saves customer settings.
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		internal static async Task UpdateSettings(CustomerSettings settings)
		{
			await ExecuteNonQueryAsync("dbo.usp_Customer_Setting_Update",
				p =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = settings.CustomerId;
					p.Add("CurrentLanguageCode", SqlDbType.NVarChar).Value = settings.CurrentLanguageCode;
					p.Add("UnknownWordMax", SqlDbType.Int).Value = settings.UnknownWordMax;
					p.Add("ShowArchivedMovies", SqlDbType.Bit).Value = settings.ShowArchivedMovies;
				});
		}

		/// <summary>
		/// Returns customer state
		/// </summary>
		/// <returns></returns>
		internal static async Task<CustomerState> GetCustomerState(int customerId)
		{
			CustomerState result = await ExecuteObjectAsync<CustomerState>("dbo.usp_CustomerState_Get",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = customerId;
				},
				(m) =>
					new CustomerState()
					{
						WorkPlace = m["WorkPlaceLayout"] as string
					}
				);

			result.CustomerId = customerId;

			return result;
		}

		/// <summary>
		/// Updates customer state.
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		internal static async Task UpdateCustomerState(CustomerState state)
		{
			await ExecuteNonQueryAsync("dbo.usp_CustomerState_Update",
				(p) =>
				{
					p.Add("CustomerId", SqlDbType.Int).Value = state.CustomerId;
					p.Add("WorkPlaceLayout", SqlDbType.NVarChar).Value = state.WorkPlace;
				});
		}

		/// <summary>
		/// Unblocks customer. If success it returns true.
		/// </summary>
		/// <param name="email"></param>
		/// <param name="confirmationCode"></param>
		/// <returns></returns>
		internal static async Task<string> Unblock(string confirmationCode)
		{
			return await ExecuteScalarAsync<string>("dbo.usp_Customer_Unblock",
				(p) =>
				{
					p.Add("ConfirmationCode", SqlDbType.NVarChar).Value = confirmationCode;
				});
		}
	}
}
