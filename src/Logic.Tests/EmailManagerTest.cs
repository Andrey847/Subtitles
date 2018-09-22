using Moq;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Tests;
using System;
using System.Configuration;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Logic.Tests
{
	/// <summary>
	/// Test for email sending.
	/// </summary>
	public class EmailManagerTest : TestBase
	{
		/// <summary>
		/// Main constructor
		/// </summary>
		public EmailManagerTest()
		{			
			EmailManager.Instance.GlobalSettings = new GlobalSettings()
			{
				BasePath = RootPath
			};
		}

		[Fact(Skip = "Contains sensetive data.")]
		public async Task SendTestMail()
		{
			EmailManager.Instance.Settings = new EmailSettings()
			{
				EmailPassword = "",
				EmailSenderAddress = "",
				EmailUseSsl = true,
				EmailUserName = "",
				 SmtpHost = "smtp.mail.ru",
				 SmtpPort = 25
			};

			await EmailManager.Instance.SendSimpleText("ag_a@mail.ru", "Hello for subtitles", "test test");
		}

	}
}
