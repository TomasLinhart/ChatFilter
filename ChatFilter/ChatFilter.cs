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
		private List<string> highlightedTexts;

		public ChatFilter()
		{
			filteredTexts = new List<string>();
			highlightedTexts = new List<string>();
		}

		public static string GetName()
		{
			return "ChatFilter";
		}

		public static int GetVersion()
		{
			return 2;
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

					if (msg.IsCommand("/filter") || msg.IsCommand("/f")) {
						String[] splitted = msg.text.Split(new char[] {' '}, 2);

						if (splitted.Length == 2) {
							AddFilter(splitted[1].ToLower());
							SendMessage("Current filters: " + string.Join(", ", filteredTexts.ToArray()));
						}

						return true;
					} else if (msg.IsCommand("/resetfilter") || msg.IsCommand("/rf")) {
						filteredTexts.Clear();
						SendMessage("Filters have been reseted");

						return true;
					} else if (msg.IsCommand("/highlight") || msg.IsCommand("/hl")) {
						String[] splitted = msg.text.Split(new char[] {' '}, 2);

						if (splitted.Length == 2) {
							AddHighlight(splitted[1].ToLower());
							SendMessage("Current highlights: " + string.Join(", ", highlightedTexts.ToArray()));
						}

						return true;
					} else if (msg.IsCommand("/resethighlight") || msg.IsCommand("/rhl")) {
						highlightedTexts.Clear();
						SendMessage("Highlights have been reseted");

						return true;
					}
				}
			} else if (info.targetMethod.Equals("ChatMessage")) {
				RoomChatMessageMessage msg = (RoomChatMessageMessage) info.arguments[0];

				msg.text = ColorizeText(msg.text);

				if (filteredTexts.Count == 0 ||
				    msg.from.ToLower() == App.MyProfile.ProfileInfo.name.ToLower() ||
				    msg.roomName.ToLower().StartsWith("trade-") ||
				    msg.from == "Scrolls" ||
				    msg.from == "ChatFilter") { // don't filter my messages
					return false;
				}

				foreach (String filteredText in filteredTexts) {
					if (msg.text.ToLower().Contains(filteredText)) {
						return false;
					}
				}

				return filteredTexts.Count > 0;
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

			foreach (String highlightedText in highlightedTexts) {
				text = text.Colorize(highlightedText, FILTER_COLOR);
			}

			return text;
		}

		public void AddFilter(string text)
		{
			filteredTexts.Add(text);
		}

		public void AddHighlight(string text)
		{
			highlightedTexts.Add(text);
		}
	}
}

