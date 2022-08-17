using System.Threading.Tasks;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public interface IOctopusCommonAsyncRepository
    {
        IEventRepository Events { get; }
        IArchivedEventFileRepository ArchivedEventFiles { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        IUserPermissionsRepository UserPermissions { get; }
        IUserTeamsRepository UserTeams { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        IUserInvitesRepository UserInvites { get; }

        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusAsyncClient Client { get; }

        RepositoryScope Scope { get; }

        /// <summary>
        /// Determines whether the specified link exists.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns>
        /// <c>true</c> if the specified link is defined; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> HasLink(string name);

        /// <summary>
        /// Determines whether the link with the specified name contains a specified parameter.
        /// </summary>
        /// <param name="linkName">The name/key of the link.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">If the link is not defined.</exception>
        Task<bool> HasLinkParameter(string linkName, string parameterName);

        /// <summary>
        /// Gets the link with the specified name.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">If the link is not defined.</exception>
        Task<string> Link(string name);
    }
}