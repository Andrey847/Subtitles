using SendGrid;
using SendGrid.Helpers.Mail;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Infrastructure;
using SubtitlesLearn.Logic.Interfaces;
using System;
using System.Collections.Generic;
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
		public EmailSettings Settings { get; set; } = new EmailSettings();

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
			bool result;

			SendGridClient client = new SendGridClient(Settings.ApiKey);

			EmailAddress from = new EmailAddress(Settings.SenderEmail);

			SendGridMessage msg = MailHelper.CreateSingleTemplateEmail(from, null, Settings.SimpleTemplateId, null);
			msg.SetGoogleAnalytics(false);
			msg.Personalizations.Clear();

			Personalization p = new Personalization();

			p.Tos = new List<EmailAddress>();
			foreach (string email in request.ToEmail.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
			{
				EmailAddress to = new EmailAddress(email);
				p.Tos.Add(to);
			}

			p.TemplateData = request;
			msg.Personalizations.Add(p);

			Response response = await client.SendEmailAsync(msg);

			string logDetails = $"To: {request.ToEmail}, Subj: {request.CommonSubject}. ";
			if (response.StatusCode != HttpStatusCode.Accepted)
			{
				// error! 
				Dictionary<string, dynamic> body = await response.DeserializeResponseBodyAsync(response.Body);

				StringBuilder error = new StringBuilder();

				if (body != null)
				{
					foreach (KeyValuePair<string, dynamic> pair in body)
					{
						error.AppendLine($"{pair.Key} - {pair.Value}");
					}
				}

				await Log.LogError("Email sending problem", $"{logDetails} Reason: {error.ToString()}");
				result = false;
			}
			else
			{
				await Log.LogInfo("Email sent", logDetails);
				result = true;
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

		/// <summary>
		/// Notifies admin about some action or error or smth like that.
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		public async Task<bool> NotifyAdmin(string subject, string body)
		{
			return await SendSimpleText(Settings.AdminEmail, $"Sub-Learn: {subject}", body);
		}

		#endregion Methods
	}
}
