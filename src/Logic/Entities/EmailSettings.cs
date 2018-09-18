namespace SubtitlesLearn.Logic.Entities
{
	/// <summary>
	/// Settings of email sending.
	/// </summary>
	public class EmailSettings
	{
		#region Email

		/// <summary>
		/// Smtp host.
		/// </summary>
		public string SmtpHost { get; set; }

		/// <summary>
		/// Smtp port.
		/// </summary>
		public int SmtpPort { get; set; }

		/// <summary>
		/// Email sender address.
		/// </summary>
		public string EmailSenderAddress { get; set; }

		/// <summary>
		/// Email user name.
		/// </summary>
		public string EmailUserName { get; set; }

		/// <summary>
		/// Email password.
		/// </summary>
		public string EmailPassword { get; set; }

		/// <summary>
		/// Email use ssl.
		/// </summary>
		public bool EmailUseSsl { get; set; }		

		#endregion Email
	}
}
