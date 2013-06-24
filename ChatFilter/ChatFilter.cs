using System;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;

namespace ChatFilter
{
	public class ChatFilter : BaseMod
	{
		private string filtertedText;

		public ChatFilter()
		{
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
					scrollsTypes["ChatRooms"].Methods.GetMethod("ChatMessage", new Type[]{ typeof(RoomChatMessageMessage) })
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

			if (info.targetMethod.Equals("ChatMessage"))
			{
				RoomChatMessageMessage msg = (RoomChatMessageMessage) info.arguments[0];

				if (msg.text.ToLower().StartsWith("/filter")) {
					String[] splitted = msg.text.Split(new char[] {' '}, 2);

					if (splitted.Length == 2) {
						filtertedText = splitted[1].ToLower();
					}

					return true;
				} else if (msg.text.ToLower().StartsWith("/resetfilter")) {
					filtertedText = null;

					return true;
				}

				if (String.IsNullOrEmpty(filtertedText) ||
				    msg.from.ToLower() == App.MyProfile.ProfileInfo.name.ToLower() || // don't filter my message
				    msg.text.ToLower().Contains(filtertedText)) {
					return false;
				}

				return true;
			}

			return false;
		}

		public override void AfterInvoke(InvocationInfo info, ref object returnValue)
		{
			return;
		}
	}
}

