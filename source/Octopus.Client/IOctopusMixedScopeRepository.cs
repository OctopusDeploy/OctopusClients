using Octopus.Client.Repositories;

namespace Octopus.Client
{
    public interface IOctopusMixedScopeRepository
    {
        IEventRepository Events { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        IUserPermissionsRepository UserPermissions { get; }
    }
}