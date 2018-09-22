using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SubtitlesLearn.Logic
{
	/// <summary>
	/// Email sending.
	/// </summary>
	public class EmailManager : ManagerBase, IEmailNotifier
	{
		#region Properties

		/// <summary>
		/// Email settings.
		/// </summary>
		public EmailSettings Settings { get; set; }

		#endregion Properties

		#region Singleton

		private static Lazy<EmailManager> _instance = new Lazy<EmailManager>(() => new EmailManager());

		/// <summary>
		/// Main class instance.
		/// </summary>
		public static EmailManager Instance => _instance.Value;

		/// <summary>
		/// Private constructor (single).
		/// </summary>
		private EmailManager()
		{

		}

		#endregion Singleton

		#region Methods

		/// <summary>
		/// Sends email to a recepient.
		/// </summary>
		/// <param name="result"></param>
		public async Task<bool> SendEmail(IEmailRequest request)
		{
			bool result = true;
			SmtpClient smtp = new SmtpClient(Settings.SmtpHost, Settings.SmtpPort);

			smtp.Credentials = new NetworkCredential(Settings.EmailUserName, Settings.EmailPassword);
			smtp.EnableSsl = Settings.EmailUseSsl;

			MailMessage msg = new MailMessage();
			msg.From = new MailAddress(Settings.EmailSenderAddress, request.From);

			foreach (string email in request.ToEmail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
			{
				msg.To.Add(email);
			}

			msg.Body = request.GetNotification();
			msg.BodyEncoding = Encoding.UTF8;
			msg.IsBodyHtml = true;

			string subject = request.GetSubject();

			// Subject limit is 100 chars
			if (subject.Length > 100)
			{
				subject = subject.Substring(0, 100);
			}

			msg.Subject = subject;

			msg.SubjectEncoding = Encoding.UTF8;

			Attachment attach = request.GetAttachment();

			if (attach != null)
			{
				msg.Attachments.Add(attach);
			}

			try
			{
				await smtp.SendMailAsync(msg);
				await Log.LogInfo("Email was sent", request.ToString());
			}
			catch (Exception ex)
			{
				result = false;
				await Log.LogInfo("Error during email sending", ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + request.ToString());
			}

			return result;
		}

		/// <summary>
		/// Sends very simple email.
		/// </summary>
		/// <param name="loginEmail"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		public async Task<bool> SendSimpleText(string to, string subject, string body)
		{
			SimpleEmailRequest request = new SimpleEmailRequest(
			to,
			subject,
			body
			);

			return await SendEmail(request);
		}

		#endregion Methods
	}
}
