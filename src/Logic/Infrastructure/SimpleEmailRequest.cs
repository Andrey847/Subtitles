using SubtitlesLearn.Logic.Interfaces;
using System.Net.Mail;
using System.Runtime.Serialization;

namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Request for the email notification.
	/// </summary>
	[DataContract]
	public class SimpleEmailRequest : IEmailRequest
	{
		#region Properties

		/// <summary>
		/// Email sender name.
		/// </summary>
		public string From { get; set; } = "Subtitles Learn";

		/// <summary>
		/// Target email address.
		/// </summary>
		public string ToEmail { get; set; }

		/// <summary>
		/// Gets notification message.
		/// </summary>
		/// <returns></returns>
		[DataMember(Name = "commonText")]
		public string CommonText { get; set; }

		/// <summary>
		/// Email subject.
		/// </summary>
		[DataMember(Name = "commonSubject")]
		public string CommonSubject { get; set; }

		#endregion Properties

		#region Construction

		/// <summary>
		/// Main constructor.
		/// </summary>
		/// <param name="toEmail"></param>
		/// <param name="transcription"></param>
		/// <param name="link"></param>
		/// <param name="wav"></param>
		public SimpleEmailRequest(string toEmail,
							string subject,
							string body
							)
		{			
			ToEmail = toEmail;
			CommonSubject = subject;
			CommonText = body;			
		}

		#endregion Construction

		#region Methods		

		public override string ToString()
		{
			return ToEmail;
		}

		/// <summary>
		/// Gets attachment list.
		/// </summary>
		/// <returns></returns>
		public Attachment GetAttachment()
		{
			return null;
		}

		#endregion Methods
	}
}