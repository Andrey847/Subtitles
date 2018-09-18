using System.ComponentModel.DataAnnotations;

namespace SubtitlesLearn.Site.Models
{
	/// <summary>
	/// Registration model.
	/// </summary>
	public class RegisterViewModel
	{
		/// <summary>
		/// Email (LoginEmail).
		/// </summary>
		[Required(ErrorMessage = "Please enter email.")]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		/// <summary>
		/// Password.
		/// </summary>
		[Required(ErrorMessage = "Please enter password")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// Password confirmation.
		/// </summary>
		[Required(ErrorMessage = "Please enter the password confirmation")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Please confirm the password")]
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// Is TOS approved?
		/// </summary>		
		public bool Tos { get; set; }
	}
}
