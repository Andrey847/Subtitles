using SubtitlesLearn.Logic.Entities;
using System.Data;
using static SubtitlesLearn.Logic.Dal.DbHelper;

namespace SubtitlesLearn.Logic.Dal
{
	/// <summary>
	/// DAL access to all user(customer) functions.
	/// </summary>
	public static class UserAccess
	{
		/// <summary>
		/// Updates customer's password.
		/// </summary>
		/// <param name="user"></param>
		public static void UpdatePassword(Customer user)
		{
			ExecuteNonQuery("dbo.usp_Customer_Password_Update", 
				(p) =>
				{
					p.Add(nameof(Customer.Email), SqlDbType.NVarChar).Value = user.Email;
					p.Add(nameof(Customer.Password), SqlDbType.NVarChar).Value = user.Password;
				});
		}
	}
}
