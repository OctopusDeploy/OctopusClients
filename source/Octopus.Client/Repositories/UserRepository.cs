using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUserRepository :
        IPaginate<UserResource>,
        IGet<UserResource>,
        IModify<UserResource>,
        IDelete<UserResource>,
        ICreate<UserResource>
    {
        UserResource Register(RegisterCommand registerCommand);
        void SignIn(LoginCommand loginCommand);
        void SignIn(string username, string password, bool rememberMe = false);
        void SignOut();
        UserResource GetCurrent();
        UserPermissionSetResource GetPermissions(UserResource user);
        ApiKeyResource CreateApiKey(UserResource user, string purpose = null);
        List<ApiKeyResource> GetApiKeys(UserResource user);
        void RevokeApiKey(ApiKeyResource apiKey);
        InvitationResource Invite(string addToTeamId);
        InvitationResource Invite(ReferenceCollection addToTeamIds);
    }
    
    class UserRepository : BasicRepository<UserResource>, IUserRepository
    {
        readonly BasicRepository<InvitationResource> invitations;

        public UserRepository(IOctopusClient client)
            : base(client, "Users")
        {
            invitations = new InvitationRepository(client);
        }

        public UserResource Register(RegisterCommand registerCommand)
        {
            Client.Post(Client.RootDocument.Link("Register"), registerCommand);
            return GetCurrent();
        }

        public void SignIn(LoginCommand loginCommand)
        {
            Client.Post(Client.RootDocument.Link("SignIn"), loginCommand);
        }

        public void SignIn(string username, string password, bool rememberMe = false)
        {
            SignIn(new LoginCommand() { Username = username, Password = password, RememberMe = rememberMe });
        }

        public void SignOut()
        {
            Client.Post(Client.RootDocument.Link("SignOut"));
        }

        public UserResource GetCurrent()
        {
            return Client.Get<UserResource>(Client.RootDocument.Link("CurrentUser"));
        }

        public UserPermissionSetResource GetPermissions(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Get<UserPermissionSetResource>(user.Link("Permissions"));
        }

        public ApiKeyResource CreateApiKey(UserResource user, string purpose = null)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Post<object, ApiKeyResource>(user.Link("ApiKeys"), new
            {
                Purpose = purpose ?? "Requested by Octopus.Client"
            });
        }

        public List<ApiKeyResource> GetApiKeys(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            var resources = new List<ApiKeyResource>();

            Client.Paginate<ApiKeyResource>(user.Link("ApiKeys"), page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public void RevokeApiKey(ApiKeyResource apiKey)
        {
            Client.Delete(apiKey.Link("Self"));
        }

        public InvitationResource Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
            return Invite(new ReferenceCollection { addToTeamId });
        }

        public InvitationResource Invite(ReferenceCollection addToTeamIds)
        {
            return invitations.Create(new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() });
        }
    }
}