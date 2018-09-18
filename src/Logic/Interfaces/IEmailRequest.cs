using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

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
		/// Gets from email.
		/// </summary>
		string From { get; }

		/// <summary>
		/// Gets notification body.
		/// </summary>
		/// <returns></returns>
		string GetNotification();

		/// <summary>
		/// Gets subject.
		/// </summary>
		/// <returns></returns>
		string GetSubject();

		/// <summary>
		/// Gets attachment (optional);
		/// </summary>
		/// <returns></returns>
		Attachment GetAttachment();
	}
}
