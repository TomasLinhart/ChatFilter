using System;

namespace ChatFilter
{
	public static class RoomChatMessageMessageExtensions
	{
		public static bool IsCommand(this RoomChatMessageMessage msg, string command)
		{
			return msg.text.ToLower().StartsWith(command);
		}
	}
}

