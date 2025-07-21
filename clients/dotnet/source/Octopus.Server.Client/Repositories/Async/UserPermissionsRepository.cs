using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserPermissionsRepository :
        ICanExtendSpaceContext<IUserPermissionsRepository>
    {
        Task<UserPermissionSetResource> Get(UserResource user);
        [Obsolete("Use GetDescriptions(UserResource) instead. This method only returns empty sets and is only kept for backwards compatibility.")]
        Task<UserPermissionSetResource> GetConfiguration(UserResource user);
        Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user);
        Task<Stream> Export(UserPermissionSetResource userPermissions);
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

        public async Task<UserPermissionSetResource> Get(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<UserPermissionSetResource>(user.Link("Permissions"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        [Obsolete("Use GetDescriptions(UserResource) instead. This method only returns empty sets and is only kept for backwards compatibility.")]
        public async Task<UserPermissionSetResource> GetConfiguration(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<UserPermissionSetResource>(user.Link("PermissionsConfiguration"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        public async Task<IReadOnlyDictionary<Permission, PermissionDescription>> GetDescriptions(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return await Client.Get<IReadOnlyDictionary<Permission, PermissionDescription>>(user.Link("PermissionsConfiguration"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        public async Task<Stream> Export(UserPermissionSetResource userPermissions)
        {
            if (userPermissions == null) throw new ArgumentNullException(nameof(userPermissions));
            return await Client.GetContent(userPermissions.Link("Export"), GetAdditionalQueryParameters()).ConfigureAwait(false);
        }

        public IUserPermissionsRepository UsingContext(SpaceContext spaceContext)
        {
            return new UserPermissionsRepository(Repository, spaceContext);
        }
    }
}