using System.ComponentModel.DataAnnotations;

namespace SubtitlesLearn.Site.Models
{
	/// <summary>
	/// Aux class for login process.
	/// </summary>
	public class LoginViewModel
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
		/// Remember me?
		/// </summary>
		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

}
