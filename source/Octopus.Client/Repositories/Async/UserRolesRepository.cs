using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IUserRolesRepository : IFindByName<UserRoleResource>, IGet<UserRoleResource>, ICreate<UserRoleResource>, IModify<UserRoleResource>, IDelete<UserRoleResource>
    {
    }

    class UserRolesRepository : BasicRepository<UserRoleResource>, IUserRolesRepository
    {
        public UserRolesRepository(IOctopusAsyncRepository repository)
            : base(repository, "UserRoles")
        {
        }

        public override async Task<UserRoleResource> Create(UserRoleResource resource, object pathParameters = null)
        {
            await RemoveInvalidPermissions(resource).ConfigureAwait(false);
            return await base.Create(resource, pathParameters).ConfigureAwait(false);
        }

        public override async Task<UserRoleResource> Modify(UserRoleResource resource)
        {
            await RemoveInvalidPermissions(resource).ConfigureAwait(false);
            return await base.Modify(resource).ConfigureAwait(false);
        }

        private async Task RemoveInvalidPermissions(UserRoleResource resource)
        {
#pragma warning disable 618
            const Permission taskViewLogPermission = Permission.TaskViewLog;
#pragma warning restore 618
            
            var versionWhenTaskViewLogWasRemoved = SemanticVersion.Parse("2019.1.7");
            var rootDocument = await Repository.LoadRootDocument().ConfigureAwait(false);
            var serverSupportsTaskViewLog = SemanticVersion.Parse(rootDocument.Version) < versionWhenTaskViewLogWasRemoved;

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
