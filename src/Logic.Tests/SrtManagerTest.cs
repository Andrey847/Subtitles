using SubtitlesLearn.Logic;
using SubtitlesLearn.Logic.Dal;
using SubtitlesLearn.Logic.Entities;
using SubtitlesLearn.Logic.Tests;
using System;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.Collections.Generic;

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
		
		/// <summary>
		/// Parses block from srt correctly.
		/// </summary>
		[Fact]
		public void ParseWords_WatermelonProblem()
		{
			string srt = @"234
00:09:15,670 --> 00:09:17,470
JIMMY: Watermelon, pickles.

235
00:09:19,510 --> 00:09:21,600
Watermelon, pickles.

236
00:09:21,620 --> 00:09:25,410
Watermelon, pickles.

237
00:09:25,420 --> 00:09:30,480
Watermelon, pickles.";

			Word[] words = SrtManager.Instance.GetWords(srt);
			// should be 1 phrase only
			string[] phrases = words.SelectMany(item => item.Phrases)
								.Select(item => item.Value)
								.Distinct()
								.ToArray();

			Assert.True(phrases.Length == 1);
		}

		/// <summary>
		/// Removing capitals from phrases.
		/// </summary>
		[Fact]
		public void ParseWords_Capitals()
		{
			Word[] words = SrtManager.Instance.SplitSentence(new Phrase(" - ERNIE: ♪ I really need you tonight ♪"));
			Assert.True(words.Length == 4);
		}

		/// <summary>
		/// Removing words in []. 
		/// </summary>
		[Fact]
		public void ParseWords_Aux1()
		{
			Word[] words = SrtManager.Instance.SplitSentence(new Phrase(" - [CHEERS AND APPLAUSE]"));
			Assert.True(words.Length == 0);
		}

		/// <summary>
		/// Removing words in []. 
		/// </summary>
		[Fact]
		public void ParseWords_Aux2()
		{
			Word[] words = SrtManager.Instance.SplitSentence(new Phrase(" - [CHUCKLING] Yeah."));
			Assert.True(words.Length == 1);
		}

		[Fact]
		public async Task GetLanguages()
		{
			Language[] langs = await SrtManager.Instance.GetLanguages();

			Assert.True(langs.Length > 0);
		}

		/// <summary>
		/// Tests setting of learned/unlerned state.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task LearnedState()
		{
			Customer customer = await UserManager.Instance.GetUser("ag_a@mail.ru");

			List<Word> words = await SrtManager.Instance.GetAllWords(customer.Id);
			Assert.NotEmpty(words);

			Word any = words.First();

			await SrtManager.Instance.SetLearned(any, true);

			// check that there is no this word
			words = await SrtManager.Instance.GetAllWords(customer.Id);
			Assert.Null(words.Where(item => item.Id == any.Id).FirstOrDefault());

			// Set it UNlearned again
			await SrtManager.Instance.SetLearned(any, false);

			words = await SrtManager.Instance.GetAllWords(customer.Id);
			Assert.NotNull(words.Where(item => item.Id == any.Id).FirstOrDefault());
		}
	}
}
