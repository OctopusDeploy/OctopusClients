using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IUserRolesRepository : IFindByName<UserRoleResource>, IGet<UserRoleResource>, ICreate<UserRoleResource>, IModify<UserRoleResource>, IDelete<UserRoleResource>
    {
    }
    
    class UserRolesRepository : BasicRepository<UserRoleResource>, IUserRolesRepository
    {
        public UserRolesRepository(IOctopusRepository repository)
            : base(repository, "UserRoles")
        {
        }
    }
}