using SubtitlesLearn.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SubtitlesLearn.Logic.Tests
{
	/// <summary>
	/// Simple tests for phrase parsing.
	/// </summary>
	public class PhraseTest
	{
		[Fact]
		public void ParseOk_1()
		{
			Phrase p = new Phrase();
			p.SetTiming("00:59:15,120 --> 00:59:16,790");

			Assert.True(p.TimeFrom == new TimeSpan(0, 0, 59, 15, 120));
			Assert.True(p.TimeTo == new TimeSpan(0, 0, 59, 16, 790));
		}
	}
}
