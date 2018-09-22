using System.ComponentModel.DataAnnotations;

namespace SubtitlesLearn.Site.Models
{
	/// <summary>
	/// Stores information of password changing.
	/// </summary>
	public class ChangePasswordViewModel
	{
		/// <summary>
		/// User login (email).
		/// </summary>
		[Required(ErrorMessage = "Please enter the email.")]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required(ErrorMessage = "Please enter password.")]
		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required(ErrorMessage = "Please enter password confirmation.")]
		[Display(Name = "Password confirmation")]
		[DataType(DataType.Password)]
		public string PasswordConfirmation { get; set; }

		/// <summary>
		/// Password.
		/// </summary>		
		[Display(Name = "Old password")]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; }

		/// <summary>
		/// Code for restoring password.
		/// </summary>		
		public string RestorePasswordCode { get; set; }
	}
}
