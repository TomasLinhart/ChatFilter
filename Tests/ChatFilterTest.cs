using System;
using NUnit.Framework;
using ChatFilter;

namespace Tests
{
	[TestFixture()]
	public class ChatFilterTest
	{
		[Test()]
		public void TestColorizeText()
		{
			ChatFilter.ChatFilter filter = new ChatFilter.ChatFilter();
			filter.AddFilter("wtb");
			filter.AddFilter("wts");
			string text = filter.ColorizeText("wtb crimson bull, wts noaidi");

			Assert.AreEqual("<color=#fde50d>wtb</color> crimson bull, <color=#fde50d>wts</color> noaidi", text);
		}

		[Test()]
		public void TestColorizeWhenGotColor()
		{
			ChatFilter.ChatFilter filter = new ChatFilter.ChatFilter();
			filter.AddFilter("a");
			string text = filter.ColorizeText("<color=#abcdef>a</color>");

			Assert.AreEqual("<color=#abcdef><color=#fde50d>a</color></color>", text);
		}

		[Test()]
		public void TestColorizeWhenGotColorWithFilterColor()
		{
			ChatFilter.ChatFilter filter = new ChatFilter.ChatFilter();
			filter.AddFilter("color");
			string text = filter.ColorizeText("<color=#abcdef>color</color>");

			Assert.AreEqual("<color=#abcdef><color=#fde50d>color</color></color>", text);
		}
	}
}

