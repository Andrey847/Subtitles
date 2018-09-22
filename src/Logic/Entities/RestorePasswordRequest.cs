using System.ComponentModel.DataAnnotations;

namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Request for restoring the passowrd.
	/// </summary>
	public class RestorePasswordRequest
	{
		/// <summary>
		/// Users email.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage = "Please enter email to send restore password link.")]
		public string Email { get; set; }

		/// <summary>
		/// For serialization.
		/// </summary>
		public RestorePasswordRequest()
		{ }

		/// <summary>
		/// Constructor to fast use.
		/// </summary>
		/// <param name="email"></param>
		public RestorePasswordRequest(string email)
		{
			Email = email;
		}
	}
}
