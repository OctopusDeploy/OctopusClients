using Octopus.Client.Model;
using Octopus.Client.Repositories;
using Octopus.Client.Exceptions;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to system-scoped parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusCommonRepository.Client" />.
    /// </summary>
    public interface IOctopusSystemRepository: IOctopusCommonRepository
    {
        ISchedulerRepository Schedulers { get; }
        IBackupRepository Backups { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        IConfigurationRepository Configuration { get; }
        IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        IMigrationRepository Migrations { get; }
        ILicensesRepository Licenses { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }
        IPerformanceConfigurationRepository PerformanceConfiguration { get; }
        IServerStatusRepository ServerStatus { get; }
        ISpaceRepository Spaces { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IUpgradeConfigurationRepository UpgradeConfiguration { get; }
        ITelemetryConfigurationRepository TelemetryConfigurationRepository { get; }

        IDeploymentFreezeRepository DeploymentFreezes { get; }

        /// <summary>
        /// Gets a document that identifies the Octopus Server (from /api) and provides links to the resources available on the
        /// server. Instead of hardcoding paths,
        /// clients should use these link properties to traverse the resources on the server. This document is lazily loaded so
        /// that it is only requested once for
        /// the current <see cref="IOctopusSystemRepository" />.
        /// </summary>
        /// <exception cref="OctopusSecurityException">
        /// HTTP 401 or 403: Thrown when the current user's API key was not valid, their
        /// account is disabled, or they don't have permission to perform the specified action.
        /// </exception>
        /// <exception cref="OctopusServerException">
        /// If any other error is successfully returned from the server (e.g., a 500
        /// server error).
        /// </exception>
        /// <exception cref="OctopusValidationException">HTTP 400: If there was a problem with the request provided by the user.</exception>
        /// <exception cref="OctopusResourceNotFoundException">HTTP 404: If the specified resource does not exist on the server.</exception>
        RootResource LoadRootDocument();
    }
}