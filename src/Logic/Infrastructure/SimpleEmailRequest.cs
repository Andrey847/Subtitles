using SubtitlesLearn.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace SubtitlesLearn.Logic.Infrastructure
{
	/// <summary>
	/// Request for the email notification.
	/// </summary>
	public class SimpleEmailRequest : IEmailRequest
	{
		#region Fields

		private string _subject;
		private string _body;

		#endregion Fields

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
		/// Message templated. Keys are {key}.
		/// </summary>
		public string Template { get; set; }

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
			_subject = subject;
			_body = body;
			Template = EmailManager.Instance.ReadFile(@"Templates\SimpleEmailTemplate.html");
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Gets notification message by supplied template.
		/// </summary>
		/// <param name="values">Key Valuye pair dictionary. Key - value to replace {key}, value - its value.</param>
		/// <returns></returns>
		private string GetNotification(Dictionary<string, string> values)
		{
			StringBuilder result = new StringBuilder(Template);

			foreach (KeyValuePair<string, string> value in values)
			{
				result.Replace($"{{{value.Key}}}", value.Value);
			}

			return result.ToString();
		}

		/// <summary>
		/// Gets notification message.
		/// </summary>
		/// <returns></returns>
		public string GetNotification()
		{
			return GetNotification(new Dictionary<string, string>()
			{
				["placeholder for the data"] = _body
			});
		}

		public override string ToString()
		{
			return ToEmail;
		}

		/// <summary>
		/// Gets subject.
		/// </summary>
		/// <returns></returns>
		public string GetSubject()
		{
			return _subject;
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