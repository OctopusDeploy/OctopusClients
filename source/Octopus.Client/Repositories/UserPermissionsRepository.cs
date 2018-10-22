using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IUserPermissionsRepository :
        ICanExtendSpaceContext<IUserPermissionsRepository>
    {
        UserPermissionSetResource Get(UserResource user);
        Stream Export(UserPermissionSetResource userPermissions);
    }
    
    class UserPermissionsRepository : MixedScopeBaseRepository<UserPermissionSetResource>, IUserPermissionsRepository
    {
        public UserPermissionsRepository(IOctopusRepository repository)
            : base(repository, null)
        {
        }

        UserPermissionsRepository(IOctopusRepository repository, SpaceContext includingSpaceContext, SpaceContext extendedSpaceContext)
            : base(repository, null, includingSpaceContext, extendedSpaceContext)
        {
        }

        public UserPermissionSetResource Get(UserResource user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return Client.Get<UserPermissionSetResource>(user.Link("Permissions"), AdditionalQueryParameters);
        }
        
        public Stream Export(UserPermissionSetResource userPermissions)
        {
            if (userPermissions == null) throw new ArgumentNullException(nameof(userPermissions));
            return Client.GetContent(userPermissions.Link("Export"), AdditionalQueryParameters);
        }

        public IUserPermissionsRepository Including(SpaceContext spaceContext)
        {
            return new UserPermissionsRepository(Repository, spaceContext, GetCurrentSpaceContext());
        }
    }
}