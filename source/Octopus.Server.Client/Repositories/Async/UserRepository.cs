using System;
using System.Collections.Generic;
using System.Threading;
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
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> FindByUsername(string username);
        Task<UserResource> FindByUsername(string username, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> Create(string username, string displayName, string password = null, string emailAddress = null);
        Task<UserResource> Create(string username, string displayName, CancellationToken cancellationToken, string password = null, string emailAddress = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> CreateServiceAccount(string username, string displayName);
        Task<UserResource> CreateServiceAccount(string username, string displayName, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> Register(RegisterCommand registerCommand);
        Task<UserResource> Register(RegisterCommand registerCommand, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SignIn(LoginCommand loginCommand);
        Task SignIn(LoginCommand loginCommand, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SignIn(string username, string password, bool rememberMe = false);
        Task SignIn(string username, string password, CancellationToken cancellationToken, bool rememberMe = false);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task SignOut();
        Task SignOut(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserResource> GetCurrent();
        Task<UserResource> GetCurrent(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<SpaceResource[]> GetSpaces(UserResource user);
        Task<SpaceResource[]> GetSpaces(UserResource user, CancellationToken cancellationToken);

        Task<GeneratedAccessTokenResource> GenerateAccessToken(UserResource user, CancellationToken cancellationToken);

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
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null);
        Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, CancellationToken cancellationToken, string purpose = null, DateTimeOffset? expires = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<ApiKeyResource>> GetApiKeys(UserResource user);
        Task<List<ApiKeyResource>> GetApiKeys(UserResource user, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task RevokeApiKey(ApiKeyResourceBase apiKey);
        Task RevokeApiKey(ApiKeyResourceBase apiKey, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task RevokeSessions(UserResource user);
        Task RevokeSessions(UserResource user, CancellationToken cancellationToken);
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserResource> FindByUsername(string username)
            => FindByUsername(username, CancellationToken.None);

        public Task<UserResource> FindByUsername(string username, CancellationToken cancellationToken)
            => FindOne(u => u.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase), path: null, pathParameters: new { filter = username }, cancellationToken: cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserResource> Create(string username, string displayName, string password = null, string emailAddress = null)
            => Create(username, displayName, CancellationToken.None, password, emailAddress);

        public Task<UserResource> Create(string username, string displayName, CancellationToken cancellationToken, string password = null, string emailAddress = null)
        {
            return Create(new UserResource
            {
                Username = username,
                DisplayName = displayName,
                Password = password,
                EmailAddress = emailAddress,
                IsActive = true,
                IsService = false
            }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserResource> CreateServiceAccount(string username, string displayName)
            => CreateServiceAccount(username, displayName, CancellationToken.None);

        public Task<UserResource> CreateServiceAccount(string username, string displayName, CancellationToken cancellationToken)
        {
            return Create(new UserResource
            {
                Username = username,
                DisplayName = displayName,
                IsActive = true,
                IsService = true
            }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserResource> Register(RegisterCommand registerCommand)
            => Register(registerCommand, CancellationToken.None);

        public async Task<UserResource> Register(RegisterCommand registerCommand, CancellationToken cancellationToken)
        {
            return await Client.Post<UserResource, UserResource>(await Repository.Link("Register").ConfigureAwait(false), registerCommand, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SignIn(LoginCommand loginCommand)
            => SignIn(loginCommand, CancellationToken.None);

        public async Task SignIn(LoginCommand loginCommand, CancellationToken cancellationToken)
        {
            await Client.SignIn(loginCommand, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SignIn(string username, string password, bool rememberMe = false)
            => SignIn(username, password, CancellationToken.None, rememberMe);

        public Task SignIn(string username, string password, CancellationToken cancellationToken, bool rememberMe = false)
        {
            return SignIn(new LoginCommand() { Username = username, Password = password, RememberMe = rememberMe }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task SignOut()
            => SignOut(CancellationToken.None);

        public Task SignOut(CancellationToken cancellationToken)
        {
            return Client.SignOut(cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserResource> GetCurrent()
            => GetCurrent(CancellationToken.None);

        public async Task<UserResource> GetCurrent(CancellationToken cancellationToken)
        {
            return await Client.Get<UserResource>(await Repository.Link("CurrentUser").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<SpaceResource[]> GetSpaces(UserResource user)
            => GetSpaces(user, CancellationToken.None);

        public Task<SpaceResource[]> GetSpaces(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Get<SpaceResource[]>(user.Link("Spaces"), cancellationToken);
        }

        public async Task<GeneratedAccessTokenResource> GenerateAccessToken(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Post<object, GeneratedAccessTokenResource>(user.Link("AccessToken"), new object(), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, string purpose = null, DateTimeOffset? expires = null)
            => CreateApiKey(user, CancellationToken.None, purpose, expires);

        public Task<ApiKeyCreatedResource> CreateApiKey(UserResource user, CancellationToken cancellationToken, string purpose = null, DateTimeOffset? expires = null)
        {
            if (user == null) throw new ArgumentNullException("user");
            return Client.Post<object, ApiKeyCreatedResource>(user.Link("ApiKeys"), new
            {
                Purpose = purpose ?? "Requested by Octopus.Client",
                Expires = expires
            }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<ApiKeyResource>> GetApiKeys(UserResource user)
            => GetApiKeys(user, CancellationToken.None);

        public async Task<List<ApiKeyResource>> GetApiKeys(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException("user");
            var resources = new List<ApiKeyResource>();

            await Client.Paginate<ApiKeyResource>(user.Link("ApiKeys"), page =>
            {
                resources.AddRange(page.Items);
                return true;
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task RevokeApiKey(ApiKeyResourceBase apiKey)
            => RevokeApiKey(apiKey, CancellationToken.None);

        public Task RevokeApiKey(ApiKeyResourceBase apiKey, CancellationToken cancellationToken)
        {
            return Client.Delete(apiKey.Link("Self"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task RevokeSessions(UserResource user)
            => RevokeSessions(user, CancellationToken.None);

        public Task RevokeSessions(UserResource user, CancellationToken cancellationToken)
        {
            return Client.Put(user.Link("RevokeSessions"), cancellationToken);
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
