using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserRepository :
        IPaginate<UserResource>,
        IGet<UserResource>,
        IModify<UserResource>,
        IDelete<UserResource>
    {
        Task<UserResource> Register(RegisterCommand registerCommand);
        Task SignIn(LoginCommand loginCommand);
        Task SignOut();
        Task<UserResource> GetCurrent();
        Task<UserPermissionSetResource> GetPermissions(UserResource user);
        Task<ApiKeyResource> CreateApiKey(UserResource user, string purpose = null);
        Task<List<ApiKeyResource>> GetApiKeys(UserResource user);
        Task RevokeApiKey(ApiKeyResource apiKey);
        Task<InvitationResource> Invite(string addToTeamId);
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds);
    }

    class UserRepository : BasicRepository<UserResource>, IUserRepository
    {
        readonly BasicRepository<InvitationResource> invitations;

        public UserRepository(IOctopusAsyncClient client)
            : base(client, "Users")
        {
            invitations = new InvitationRepository(client);
        }

        public async Task<UserResource> Register(RegisterCommand registerCommand)
        {
            await Client.Post(Client.RootDocument.Link("Register"), registerCommand).ConfigureAwait(false);
            return await GetCurrent().ConfigureAwait(false);
        }

        public Task SignIn(LoginCommand loginCommand)
        {
            return Client.Post(Client.RootDocument.Link("SignIn"), loginCommand);
        }

        public Task SignOut()
        {
            return Client.Post(Client.RootDocument.Link("SignOut"));
        }

        public Task<UserResource> GetCurrent()
        {
            return Client.Get<UserResource>(Client.RootDocument.Link("CurrentUser"));
        }

        public Task<UserPermissionSetResource> GetPermissions(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Get<UserPermissionSetResource>(user.Link("Permissions"));
        }

        public Task<ApiKeyResource> CreateApiKey(UserResource user, string purpose = null)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Post<object, ApiKeyResource>(user.Link("ApiKeys"), new
            {
                Purpose = purpose ?? "Requested by Octopus.Client"
            });
        }

        public async Task<List<ApiKeyResource>> GetApiKeys(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var resources = new List<ApiKeyResource>();

            await Client.Paginate<ApiKeyResource>(user.Link("ApiKeys"), page =>
            {
                resources.AddRange(page.Items);
                return true;
            }).ConfigureAwait(false);

            return resources;
        }

        public Task RevokeApiKey(ApiKeyResource apiKey)
        {
            return Client.Delete(apiKey.Link("Self"));
        }

        public Task<InvitationResource> Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
            return Invite(new ReferenceCollection { addToTeamId });
        }

        public Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
        {
            return invitations.Create(new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() });
        }
    }
}
