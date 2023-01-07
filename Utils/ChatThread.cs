using System;
using Azure.Core;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;

namespace ChatBackend.Utils
{
	public class ACSChatThread
	{

		public ChatClient ParentChatClient { get; private set; }
		public ChatParticipant? Moderator { get; private set; }
		public ChatThreadClient? Client { get; private set; }
		public string ThreadId { get; private set; }

		public ACSChatThread(ChatClient chatClient, ChatParticipant? moderator)
		{
			ParentChatClient = chatClient;
			Moderator = moderator;
		}


		public string CreateThread(string topic)
		{
			if(Moderator == null)
            {
				throw new Exception("モデレーターがセットされていない！");
            }
			var res = ParentChatClient.CreateChatThread(
				topic: topic,
				participants: new[] { Moderator }
			);
			GetThread(res.Value.ChatThread.Id);
			return ThreadId;
		}

		public string GetThread(string threadId)
		{
			Client = ParentChatClient.GetChatThreadClient(threadId);
			ThreadId = Client.Id;
			return ThreadId;
		}

		// new[] { participant1, participant2, ... }
		public void AddParticipants(ChatParticipant[] listOfUserAsParticipant)
		{
			if(Client == null)
			{
				throw new Exception("スレッドがまだ選択されていない！");
			}
			Client.AddParticipants(listOfUserAsParticipant);

		}

	}
}

