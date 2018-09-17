using System;
using System.Collections.Generic;
using System.IO;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IUserPermissionsRepository :
        ICanLimitToSpaces<IUserPermissionsRepository>
    {
        UserPermissionSetResource Get(UserResource user);
        Stream Export(UserPermissionSetResource userPermissions);
    }
    
    class UserPermissionsRepository : MixedScopeBaseRepository<UserPermissionSetResource>, IUserPermissionsRepository
    {
        public UserPermissionsRepository(IOctopusClient client)
            : base(client, null, null)
        {
        }

        UserPermissionsRepository(IOctopusClient client, SpaceQueryContext spaceQueryContext)
            : base(client, null, spaceQueryContext)
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

        public IUserPermissionsRepository LimitTo(bool includeSystem, params string[] spaceIds)
        {
            return new UserPermissionsRepository(Client, CreateSpaceQueryContext(includeSystem, spaceIds));
        }
    }
}