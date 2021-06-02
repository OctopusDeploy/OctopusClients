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
        IDelete<UserResource>,
        ICreate<UserResource>
    {
        Task<UserResource> FindByUsername(string username);
        Task<UserResource> Create(string username, string displayName, string password = null, string emailAddress = null);
        Task<UserResource> CreateServiceAccount(string username, string displayName);
        Task<UserResource> Register(RegisterCommand registerCommand);
        Task SignIn(LoginCommand loginCommand);
        Task SignIn(string username, string password, bool rememberMe = false);
        Task SignOut();
        Task<UserResource> GetCurrent();
        Task<SpaceResource[]> GetSpaces(UserResource user);
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
        Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null);
        Task<List<ApiKeyResource>> GetApiKeys(UserResource user);
        Task RevokeApiKey(ApiKeyResourceBase apiKey);
        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        Task<InvitationResource> Invite(string addToTeamId);
        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        Task<InvitationResource> Invite(ReferenceCollection addToTeamIds);
    }

    class UserRepository : BasicRepository<UserResource>, IUserRepository
    {
        readonly BasicRepository<InvitationResource> invitations;

        public UserRepository(IOctopusAsyncRepository repository)
            : base(repository, "Users")
        {
            invitations = new LegacyInvitationRepository(Repository);
        }

        public Task<UserResource> FindByUsername(string username) 
            => FindOne(u => u.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase), pathParameters: new {filter = username});

        public Task<UserResource> Create(string username, string displayName, string password = null, string emailAddress = null)
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

        public Task<UserResource> CreateServiceAccount(string username, string displayName)
        {
            return Create(new UserResource
            {
                Username = username,
                DisplayName = displayName,
                IsActive = true,
                IsService = true
            });
        }

        public async Task<UserResource> Register(RegisterCommand registerCommand)
        {
            return await Client.Post<UserResource,UserResource>(await Repository.Link("Register").ConfigureAwait(false), registerCommand).ConfigureAwait(false);
        }

        public async Task SignIn(LoginCommand loginCommand)
        {
            await Client.SignIn(loginCommand).ConfigureAwait(false);
        }

        public Task SignIn(string username, string password, bool rememberMe = false)
        {
            return SignIn(new LoginCommand() {Username = username, Password = password, RememberMe = rememberMe});
        }

        public Task SignOut()
        {
            return Client.SignOut();
        }

        public async Task<UserResource> GetCurrent()
        {
            return await Client.Get<UserResource>(await Repository.Link("CurrentUser").ConfigureAwait(false)).ConfigureAwait(false);
        }

        public Task<SpaceResource[]> GetSpaces(UserResource user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Get<SpaceResource[]>(user.Link("Spaces"));
        }

        public Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Post<object, ApiKeyCreatedResource>(user.Link("ApiKeys"), new
            {
                Purpose = purpose ?? "Requested by Octopus.Client",
                Expires = expires
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

        public Task RevokeApiKey(ApiKeyResourceBase apiKey)
        {
            return Client.Delete(apiKey.Link("Self"));
        }

        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        public Task<InvitationResource> Invite(string addToTeamId)
        {
            if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
            return Invite(new ReferenceCollection { addToTeamId });
        }

        [Obsolete("Use the " + nameof(IUserInvitesRepository) + " instead", false)]
        public Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
        {
            return invitations.Create(new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() });
        }
    }
}
