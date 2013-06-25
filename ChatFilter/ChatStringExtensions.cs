using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatFilter
{
	public static class ChatStringExtensions
	{
		private const string COLOR_TAG_START = "<color=#{0}>";
		private const string COLOR_TAG_END = "</color>";

		public static string WithColor(this string text, string color)
		{
			return string.Format(text, color);
		}

		public static string Colorize(this string text, string color)
		{
			return COLOR_TAG_START.WithColor(color) + text + COLOR_TAG_END;
		}

		public static string Colorize(this string text, string what, string color)
		{
			return Regex.Replace(text, Regex.Escape(what), COLOR_TAG_START.WithColor(color) + what + COLOR_TAG_END, RegexOptions.IgnoreCase);
		}
	}
}

