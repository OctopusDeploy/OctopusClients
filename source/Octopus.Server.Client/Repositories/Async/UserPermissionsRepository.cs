using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserPermissionsRepository :
        ICanExtendSpaceContext<IUserPermissionsRepository>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<UserPermissionSetResource> Get(UserResource user);
        Task<UserPermissionSetResource> Get(UserResource user, CancellationToken cancellationToken);
        [Obsolete("Use GetDescriptions(UserResource) instead. This method only returns empty sets and is only kept for backwards compatibility.")]
        Task<UserPermissionSetResource> GetConfiguration(UserResource user);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user);
        Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> Export(UserPermissionSetResource userPermissions);
        Task<Stream> Export(UserPermissionSetResource userPermissions, CancellationToken cancellationToken);
    }

    class UserPermissionsRepository : MixedScopeBaseRepository<UserPermissionSetResource>, IUserPermissionsRepository
    {
        public UserPermissionsRepository(IOctopusAsyncRepository repository)
            : base(repository, null)
        {
        }

        UserPermissionsRepository(IOctopusAsyncRepository repository, SpaceContext spaceContext)
            : base(repository, null, spaceContext)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<UserPermissionSetResource> Get(UserResource user)
            => Get(user, CancellationToken.None);

        public async Task<UserPermissionSetResource> Get(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<UserPermissionSetResource>(user.Link("Permissions"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Use GetDescriptions(UserResource) instead. This method only returns empty sets and is only kept for backwards compatibility.")]
        public async Task<UserPermissionSetResource> GetConfiguration(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<UserPermissionSetResource>(user.Link("PermissionsConfiguration"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user)
            => GetDescriptions(user, CancellationToken.None);

        public async Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user, CancellationToken cancellationToken)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<IReadOnlyDictionary<Permission, PermissionDescription>>(user.Link("PermissionsConfiguration"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> Export(UserPermissionSetResource userPermissions)
            => Export(userPermissions, CancellationToken.None);

        public async Task<Stream> Export(UserPermissionSetResource userPermissions, CancellationToken cancellationToken)
        {
            if (userPermissions == null) throw new ArgumentNullException(nameof(userPermissions));
            return await Client.GetContent(userPermissions.Link("Export"), GetAdditionalQueryParameters(), cancellationToken).ConfigureAwait(false);
        }

        public IUserPermissionsRepository UsingContext(SpaceContext spaceContext)
        {
            return new UserPermissionsRepository(Repository, spaceContext);
        }
    }
}
