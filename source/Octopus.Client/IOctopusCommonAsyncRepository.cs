using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public interface IOctopusCommonAsyncRepository
    {
        IEventRepository Events { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        IUserPermissionsRepository UserPermissions { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
    }
}