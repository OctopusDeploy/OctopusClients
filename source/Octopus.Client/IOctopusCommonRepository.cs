using Octopus.Client.Repositories;

namespace Octopus.Client
{
    public interface IOctopusCommonRepository
    {
        IEventRepository Events { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        IUserPermissionsRepository UserPermissions { get; }
        IUserInvitesRepository UserInvites { get; }

        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusClient Client { get; }
        RepositoryScope Scope { get; }

        /// <summary>
        /// Determines whether the specified link exists.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns>
        /// <c>true</c> if the specified link is defined; otherwise, <c>false</c>.
        /// </returns>
        bool HasLink(string name);

        /// <summary>
        /// Gets the link with the specified name.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">If the link is not defined.</exception>
        string Link(string name);
    }
}
