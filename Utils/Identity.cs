using System;
using Azure.Core;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Communication.Chat;

namespace ChatBackend.Utils
{
	public class ACSIdentity
    {
        public string ConnectionString;
        private CommunicationIdentityClient client;

		public CommunicationUserIdentifier? Identity { get; private set; }
		public string? RawId => Identity?.RawId;
		public ChatParticipant ChatParticipant { get; private set; }

		public ACSIdentity(string acsConnectionString)
		{
			ConnectionString = acsConnectionString;
			client = new CommunicationIdentityClient(ConnectionString);
		}

        public async Task<CommunicationUserIdentifier> CreateIdentity()
		{
			var idRes = await client.CreateUserAsync();
			Identity = idRes.Value;
			return Identity;
		}

		public async Task<AccessToken> CreateTokenAsync()
		{
			var tknRes = await client.GetTokenAsync(
				await CreateIdentity(),
				scopes: new[] {
					CommunicationTokenScope.VoIP,
					CommunicationTokenScope.Chat
				}
			);
			AccessToken tkn = tknRes.Value;
			return tkn;
		}

		public async Task<AccessToken> GetRefleshedTokenAsync(string? identityRawId = null)
		{
			string? _id = identityRawId != null
				? identityRawId
				: RawId;
			if(_id == null)
			{
				throw new Exception("no!");
			}
			Identity = new CommunicationUserIdentifier(_id);
			var tknRes = await client.GetTokenAsync(
				Identity,
				scopes: new[] {
					CommunicationTokenScope.VoIP,
					CommunicationTokenScope.Chat
				}
			);
			AccessToken tkn = tknRes.Value;
			return tkn;
		}

		public ChatParticipant GenerateChatParticipant(string displayName)
		{
			ChatParticipant = new ChatParticipant(Identity) { DisplayName = displayName };
			return ChatParticipant;
		}
	}
}

