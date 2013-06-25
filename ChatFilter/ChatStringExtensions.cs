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
			int start = text.IndexOf(what, 0, StringComparison.InvariantCultureIgnoreCase);
			while (start != -1) {
				StringBuilder builder = new StringBuilder();
				if (start != 0) {
					builder.Append(text.Substring(0, start));
				}
				builder.Append(COLOR_TAG_START.WithColor(color));
				builder.Append(text.Substring(start, what.Length));
				builder.Append(COLOR_TAG_END);
				builder.Append(text.Substring(start + what.Length, text.Length - start - what.Length));
				text = builder.ToString();
				start = text.IndexOf(what, start + COLOR_TAG_START.WithColor(color).Length + what.Length, StringComparison.InvariantCultureIgnoreCase);
			}

			return text;
		}
	}
}

