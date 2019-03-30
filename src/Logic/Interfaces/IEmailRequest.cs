using System.Net.Mail;

namespace SubtitlesLearn.Logic.Interfaces
{
	/// <summary>
	/// Common interface for all email requests.
	/// </summary>
	public interface IEmailRequest
	{
		/// <summary>
		/// Gets ToEmail.
		/// </summary>
		string ToEmail { get; }

		/// <summary>
		/// Gets notification body.
		/// /// </summary>
		/// <returns></returns>
		/// <remarks>Do not change this property as it is used for handlebars of sendgrid.</remarks>
		string CommonText { get; set; }

		/// <summary>
		/// Gets subject.
		/// 
		/// </summary>
		/// <returns></returns>
		/// /// <remarks>Do not change this property as it is used for handlebars of sendgrid.</remarks>
		string CommonSubject { get; set; }		

		/// <summary>
		/// Gets attachment (optional);
		/// </summary>
		/// <returns></returns>
		Attachment GetAttachment();
	}
}
