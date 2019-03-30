namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Settings of email sending.
	/// </summary>
	public class EmailSettings
	{
		#region Email

		/// <summary>
		/// Admin email.
		/// </summary>
		public string AdminEmail { get; set; }	

		/// <summary>
		/// Sendgrid api key.
		/// </summary>
		public string ApiKey { get; set; }		

		/// <summary>
		/// Template id for simple emails.
		/// </summary>
		public string SimpleTemplateId { get; set; }

		/// <summary>
		/// Sender email address.
		/// </summary>
		public string SenderEmail { get; set; }

		#endregion Email
	}
}
