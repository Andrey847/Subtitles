using Moq;
using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Tests;
using System.Threading.Tasks;
using Xunit;

namespace SubtitlesLearn.Logic.Tests
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

			EmailManager.Instance.Log = new Mock<ILogging>().Object;
		}

		[Fact(Skip = "Contains sensetive data. Used for real sendgrid integration testing only")]
		public async Task SendTestMail()
		{
			EmailManager.Instance.Settings = new EmailSettings()
			{
				AdminEmail = "ag_a@mail.ru",
				ApiKey = "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",
				SimpleTemplateId = "d-dfad3dcda8e24a65936d8b9fab92dc9a",
				SenderEmail = "info@subtitleslearn.com"
			};

			bool success = await EmailManager.Instance.SendSimpleText("ag_a@mail.ru", "Hello for subtitles", "test test");

			Assert.True(success, "Unable to send email.");
		}

	}
}
