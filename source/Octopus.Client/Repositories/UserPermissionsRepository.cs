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
        public UserPermissionsRepository(IOctopusClient client)
            : base(client, null, null)
        {
        }

        UserPermissionsRepository(IOctopusClient client, SpaceContext spaceContext)
            : base(client, null, spaceContext)
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
            return new UserPermissionsRepository(Client, base.ExtendSpaceContext(spaceContext));
        }
    }
}