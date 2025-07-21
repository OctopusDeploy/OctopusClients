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
        UserResource FindByUsername(string username);
        UserResource Create(string username, string displayName, string password = null, string emailAddress = null);
        UserResource CreateServiceAccount(string username, string displayName);
        UserResource Register(RegisterCommand registerCommand);
        void SignIn(LoginCommand loginCommand);
        void SignIn(string username, string password, bool rememberMe = false);
        void SignOut();
        UserResource GetCurrent();
        SpaceResource[] GetSpaces(UserResource user);
        /// <summary>
        /// Creates a new API key for a user.
        /// </summary>
        /// <param name="user">The user to create the key for.</param>
        /// <param name="purpose">The purpose of the API key.</param>
        /// <param name="expires">The expiry date of the key. If null, the key will never expire.</param>
        /// <returns>The newly created API key resource.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="user"/> was null.
        /// </exception>
        ApiKeyCreatedResource CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null);
        List<ApiKeyResource> GetApiKeys(UserResource user);
        void RevokeApiKey(ApiKeyResourceBase apiKey);
        void RevokeSessions(UserResource user);
        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        InvitationResource Invite(string addToTeamId);
        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        InvitationResource Invite(ReferenceCollection addToTeamIds);
    }
    
    class UserRepository : BasicRepository<UserResource>, IUserRepository
    {
        readonly BasicRepository<InvitationResource> invitations;

        public UserRepository(IOctopusRepository repository)
            : base(repository, "Users")
        {
            invitations = new LegacyInvitationRepository(repository);
        }

        public UserResource FindByUsername(string username) 
            => FindOne(u => u.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase), pathParameters: new {filter = username});

        public UserResource Create(string username, string displayName, string password = null, string emailAddress = null)
        {
            return Create(new UserResource
            {
                Username = username,
                DisplayName = displayName,
                Password = password,
                EmailAddress = emailAddress,
                IsActive = true,
                IsService = false
            });
        }

        public UserResource CreateServiceAccount(string username, string displayName)
        {
            return Create(new UserResource
            {
                Username = username,
                DisplayName = displayName,
                IsActive = true,
                IsService = true
            });
        }
        public UserResource Register(RegisterCommand registerCommand)
        {
            return Client.Post<UserResource, UserResource>(Repository.Link("Register"), registerCommand);
        }

        public void SignIn(LoginCommand loginCommand)
        {
           Client.SignIn(loginCommand);
        }

        public void SignIn(string username, string password, bool rememberMe = false)
        {
            SignIn(new LoginCommand() { Username = username, Password = password, RememberMe = rememberMe });
        }

        public void SignOut()
        {
            Client.SignOut();
        }

        public UserResource GetCurrent()
        {
            return Client.Get<UserResource>(Repository.Link("CurrentUser"));
        }
        
        public SpaceResource[] GetSpaces(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Get<SpaceResource[]>(user.Link("Spaces"));
        }

        public ApiKeyCreatedResource CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Post<object, ApiKeyCreatedResource>(user.Link("ApiKeys"), new
            {
                Purpose = purpose ?? "Requested by Octopus.Client",
                Expires = expires
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

        public void RevokeApiKey(ApiKeyResourceBase apiKey)
        {
            Client.Delete(apiKey.Link("Self"));
        }

        public void RevokeSessions(UserResource user)
        {
            Client.Put(user.Link("RevokeSessions"));
        }

        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        public InvitationResource Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
            return Invite(new ReferenceCollection { addToTeamId });
        }

        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        public InvitationResource Invite(ReferenceCollection addToTeamIds)
        {
            return invitations.Create(new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() });
        }
    }
}