using System.Collections.Generic;
using System.Linq;
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

        public override UserRoleResource Create(UserRoleResource resource, object pathParameters = null)
        {
            RemoveInvalidPermissions(resource);
            return base.Create(resource, pathParameters);
        }

        public override UserRoleResource Modify(UserRoleResource resource)
        {
            RemoveInvalidPermissions(resource);
            return base.Modify(resource);
        }

        private void RemoveInvalidPermissions(UserRoleResource resource)
        {
#pragma warning disable 618
            const Permission taskViewLogPermission = Permission.TaskViewLog;
#pragma warning restore 618
            
            var versionWhenTaskViewLogWasRemoved = SemanticVersion.Parse("2019.1.7");
            var serverSupportsTaskViewLog = SemanticVersion.Parse(Repository.LoadRootDocument().Version) < versionWhenTaskViewLogWasRemoved;

            if (!serverSupportsTaskViewLog)
            {
                resource.GrantedSpacePermissions = RemoveDeprecatedPermission(taskViewLogPermission, resource.GrantedSpacePermissions);
                resource.GrantedSystemPermissions = RemoveDeprecatedPermission(taskViewLogPermission, resource.GrantedSystemPermissions);
            }

            List<Permission> RemoveDeprecatedPermission(Permission permissionToRemove, List<Permission> permissions)
            {
                return permissions?.Where(p => p != permissionToRemove).ToList();
            }
        }
    }
}