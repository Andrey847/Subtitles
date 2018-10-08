using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Tests;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Logic.Tests
{
	/// <summary>
	/// Tests for user management.
	/// </summary>
	public class SrtManagerTest : TestBase
	{
		/// <summary>
		/// Default constructor (can be used for initialization).
		/// </summary>
		public SrtManagerTest()
		{			
		}

		/// <summary>
		/// Parses block from srt correctly.
		/// </summary>
		[Fact]
		public void ParseWords_1()
		{
			string srt = @"120
00:04:55,186 --> 00:04:57,723
You're supposed to watch a
baby for five minutes in a car.";

			Word[] words = SrtManager.Instance.GetWords(srt);

			// 7 words as words with ' are removed (You're for example)
			Assert.True(words.Length == 7);

			// should be 1 phrase only
			string[] phrases = words.SelectMany(item => item.Phrases)
								.Select(item => item.Value)
								.Distinct()
								.ToArray();

			Assert.True(phrases.Length == 1);
		}

		/// <summary>
		/// Parses block from srt correctly.
		/// </summary>
		[Fact]
		public void ParseWords_2()
		{
			string srt = @"133
00:05:31,456 --> 00:05:33,389
- Shirley, I can't! Shirley, that's not...
- A-All right, all right.";

			Word[] words = SrtManager.Instance.GetWords(srt);
			// should be 2 phrase only
			string[] phrases = words.SelectMany(item => item.Phrases)
								.Select(item => item.Value)
								.Distinct()
								.ToArray();

			Assert.True(phrases.Length == 2);
		}

		[Fact]
		public async Task GetLanguages()
		{
			Language[] langs = await SrtManager.Instance.GetLanguages();

			Assert.True(langs.Length > 0);
		}
	}
}
