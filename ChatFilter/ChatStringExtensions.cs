using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ChatFilter
{
	public static class ChatStringExtensions
	{
		private const string COLOR_TAG_START = "<color=#{0}>";
		private const int COLOR_TAG_START_LENGTH = 15;
		private const string COLOR_TAG_END = "</color>";
		private const int COLOR_TAG_END_LENGTH = 8;

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
					int afterTagEnd = InColorTag(start, text);
					if (afterTagEnd != -1) {
						start = text.IndexOf(what, afterTagEnd);
						continue;
					}
				}
				builder.Append(COLOR_TAG_START.WithColor(color));
				builder.Append(text.Substring(start, what.Length));
				builder.Append(COLOR_TAG_END);
				builder.Append(text.Substring(start + what.Length, text.Length - start - what.Length));
				text = builder.ToString();
				start = text.IndexOf(what, start + COLOR_TAG_START_LENGTH + what.Length + COLOR_TAG_END_LENGTH, StringComparison.InvariantCultureIgnoreCase);
			}

			return text;
		}

		private static int InColorTag(int start, string text)
		{
			int tagStart = 0;
			for (int i = start; i < text.Length; i++) {
				if (text[i] == '>') {
					tagStart = i + 1;
					break;
				} else if (text[i] == '<') {
					return -1;
				}
			}
			MatchCollection mc = Regex.Matches(text, @"<color=#[0-9a-fA-F]{6}>");
			foreach (Match m in mc) {
				if (m.Index == tagStart - COLOR_TAG_START_LENGTH) {
					return tagStart;
				}
			}

			mc = Regex.Matches(text, @"</color>");
			foreach (Match m in mc) {
				if (m.Index == tagStart - COLOR_TAG_END_LENGTH) {
					return tagStart;
				}
			}

			return -1;
		}
	}
}

