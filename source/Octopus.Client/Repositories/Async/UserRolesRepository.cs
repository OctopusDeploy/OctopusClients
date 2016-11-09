using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserRolesRepository : IFindByName<UserRoleResource>, IGet<UserRoleResource>
    {
    }

    class UserRolesRepository : BasicRepository<UserRoleResource>, IUserRolesRepository
    {
        public UserRolesRepository(IOctopusAsyncClient client)
            : base(client, "UserRoles")
        {
        }
    }
}
