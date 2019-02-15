using System.Threading;
using Octopus.Client.Exceptions;
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
            AssertRoleContainsValidPermissions(resource);
            return base.Create(resource, pathParameters);
        }

        public override UserRoleResource Modify(UserRoleResource resource)
        {
            AssertRoleContainsValidPermissions(resource);
            return base.Modify(resource);
        }

        private void AssertRoleContainsValidPermissions(UserRoleResource resource)
        {
#pragma warning disable 618
            const Permission taskViewLogPermission = Permission.TaskViewLog;
#pragma warning restore 618
            
            var roleUsesTaskViewLog = resource.GrantedSpacePermissions.Contains(taskViewLogPermission) ||
                                      resource.GrantedSystemPermissions.Contains(taskViewLogPermission);
            var versionWhenTaskViewLogWasRemoved = SemanticVersion.Parse("2019.1.7");
            var serverSupportsTaskViewLog = SemanticVersion.Parse(Repository.LoadRootDocument().Version) < versionWhenTaskViewLogWasRemoved;
            if (roleUsesTaskViewLog && !serverSupportsTaskViewLog)
            {
                throw new PermissionNotSupportedException(taskViewLogPermission);
            }
        }
    }
}