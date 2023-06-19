using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            MinimumCompatibleVersion("2019.1.0");
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public override async Task<UserRoleResource> Create(UserRoleResource resource, object pathParameters = null)
            => await Create(resource, pathParameters, CancellationToken.None);
        
        public override async Task<UserRoleResource> Create(UserRoleResource resource, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            await RemoveInvalidPermissions(resource, cancellationToken).ConfigureAwait(false);
            return await base.Create(resource, pathParameters, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public override async Task<UserRoleResource> Modify(UserRoleResource resource)
            => await Modify(resource, CancellationToken.None);
        
        public override async Task<UserRoleResource> Modify(UserRoleResource resource, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            await RemoveInvalidPermissions(resource, cancellationToken).ConfigureAwait(false);
            return await base.Modify(resource, cancellationToken).ConfigureAwait(false);
        }

#pragma warning disable 618
        static readonly Permission TaskViewLogPermission = Permission.TaskViewLog;
#pragma warning restore 618

        private async Task RemoveInvalidPermissions(UserRoleResource resource, CancellationToken cancellationToken)
        {
            var versionWhenTaskViewLogWasRemoved = SemanticVersion.Parse("2019.1.7");
            var rootDocument = await Repository.LoadRootDocument(cancellationToken).ConfigureAwait(false);
            var serverSupportsTaskViewLog = SemanticVersion.Parse(rootDocument.Version) < versionWhenTaskViewLogWasRemoved;

            if (!serverSupportsTaskViewLog)
            {
                resource.GrantedSpacePermissions = RemoveDeprecatedPermission(TaskViewLogPermission, resource.GrantedSpacePermissions);
                resource.GrantedSystemPermissions = RemoveDeprecatedPermission(TaskViewLogPermission, resource.GrantedSystemPermissions);
            }

            List<Permission> RemoveDeprecatedPermission(Permission permissionToRemove, List<Permission> permissions)
            {
                return permissions?.Where(p => p != permissionToRemove).ToList();
            }
        }
    }
}
