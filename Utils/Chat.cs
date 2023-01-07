using System;
using Azure.Core;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;

namespace ChatBackend.Utils
{
	public class ACSChat
	{
		private CommunicationTokenCredential credential;
        public Uri endpoint;

		public ACSChat(Uri acsEndpointUri, AccessToken tkn)
		{
			credential = GenerateTokenCredential(tkn);
			endpoint = acsEndpointUri;
			CreateChatClient();
		}

        public ChatClient Client { get; private set; }

        public CommunicationTokenCredential GenerateTokenCredential(AccessToken tkn)
		{
			return new CommunicationTokenCredential(tkn.Token);
		}

		public ChatClient CreateChatClient(CommunicationTokenCredential? acsCredential = null)
		{
			CommunicationTokenCredential cred = acsCredential != null
				? acsCredential
				: credential;
			Client = new ChatClient(endpoint, cred);
			return Client;
		}


	}


}

