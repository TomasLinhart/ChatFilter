using System;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;
using System.Collections.Generic;
using System.Text;

namespace ChatFilter
{
	public class ChatFilter : BaseMod
	{
		private const string INFO_COLOR = "aa803f";
		private const string FILTER_COLOR = "fde50d";

		private List<string> filteredTexts;

		public ChatFilter()
		{
			filteredTexts = new List<string>();
		}

		public static string GetName()
		{
			return "ChatFilter";
		}

		public static int GetVersion()
		{
			return 1;
		}

		public static MethodDefinition[] GetHooks(TypeDefinitionCollection scrollsTypes, int version)
		{
			try
			{
				return new MethodDefinition[] {
					scrollsTypes["ChatRooms"].Methods.GetMethod("ChatMessage", new Type[]{ typeof(RoomChatMessageMessage) }),
					scrollsTypes["Communicator"].Methods.GetMethod("sendRequest", new Type[]{ typeof(Message) })
				};
			} 
			catch
			{
				return new MethodDefinition[] {};
			}
		}

		public override bool BeforeInvoke(InvocationInfo info, out object returnValue)
		{
			returnValue = null;

			if (info.targetMethod.Equals("sendRequest"))
			{
				if (info.arguments[0] is RoomChatMessageMessage)
				{
					RoomChatMessageMessage msg = (RoomChatMessageMessage) info.arguments[0];

					if (msg.text.ToLower().StartsWith("/filter")) {
						String[] splitted = msg.text.Split(new char[] {' '}, 2);

						if (splitted.Length == 2) {
							AddFilter(splitted[1].ToLower());
							SendMessage("Current filters: " + string.Join(", ", filteredTexts.ToArray()));
						}

						return true;
					} else if (msg.text.ToLower().StartsWith("/resetfilter")) {
						filteredTexts.Clear();
						SendMessage("Filters have been reseted");

						return true;
					}
				}
			}

			if (info.targetMethod.Equals("ChatMessage")) {
				RoomChatMessageMessage msg = (RoomChatMessageMessage) info.arguments[0];

				if (filteredTexts.Count == 0 ||
				    msg.from.ToLower() == App.MyProfile.ProfileInfo.name.ToLower()) { // don't filter my message
					return false;
				}

				foreach (String filteredText in filteredTexts) {
					if (msg.text.ToLower().Contains(filteredText)) {
						msg.text = ColorizeText(msg.text);
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public override void AfterInvoke(InvocationInfo info, ref object returnValue)
		{
			return;
		}

		protected void SendMessage(string message)
		{
			RoomChatMessageMessage msg = new RoomChatMessageMessage();
			msg.from = GetName();
			msg.text = message.Colorize(INFO_COLOR);
			msg.roomName = App.ArenaChat.ChatRooms.GetCurrentRoom();

			App.ChatUI.handleMessage(msg);
			App.ArenaChat.ChatRooms.ChatMessage(msg);
		}

		public string ColorizeText(string text)
		{
			foreach (String filteredText in filteredTexts) {
				text = text.Colorize(filteredText, FILTER_COLOR);
			}

			return text;
		}

		public void AddFilter(string filteredText)
		{
			filteredTexts.Add(filteredText);
		}
	}
}

