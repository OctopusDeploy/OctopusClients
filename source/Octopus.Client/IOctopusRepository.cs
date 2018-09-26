#if SYNC_CLIENT
using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusRepository.Client" />.
    /// </summary>
    public interface IOctopusRepository
    {
        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusClient Client { get; }

        IAccountRepository Accounts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        IArtifactRepository Artifacts { get; }
        IBackupRepository Backups { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        ICertificateConfigurationRepository CertificateConfiguration { get; }
        ICertificateRepository Certificates { get; }
        IChannelRepository Channels { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates {get; }
        IConfigurationRepository Configuration { get; }
        IDashboardConfigurationRepository DashboardConfigurations { get; }
        IDashboardRepository Dashboards { get; }
        IDefectsRepository Defects { get; }
        IDeploymentProcessRepository DeploymentProcesses { get; }
        IDeploymentRepository Deployments { get; }
        IEnvironmentRepository Environments { get; }
        IEventRepository Events { get; }
        IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        IFeedRepository Feeds { get; }
        IInterruptionRepository Interruptions { get; }
        ILibraryVariableSetRepository LibraryVariableSets { get; }
        ILifecyclesRepository Lifecycles { get; }
        IMachinePolicyRepository MachinePolicies { get; }
        IMachineRepository Machines { get; }
        IMachineRoleRepository MachineRoles { get; }
        IMigrationRepository Migrations { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }
        IPerformanceConfigurationRepository PerformanceConfiguration { get; }
        IProjectGroupRepository ProjectGroups { get; }
        IProjectRepository Projects { get; }
        IProjectTriggerRepository ProjectTriggers { get; }
        IProxyRepository Proxies { get; }
        IReleaseRepository Releases { get; }
        IRetentionPolicyRepository RetentionPolicies { get; }
        ISchedulerRepository Schedulers { get; }
        IServerStatusRepository ServerStatus { get; }
        ISpaceRepository Spaces { get; }
        ISubscriptionRepository Subscriptions { get; }
        ITagSetRepository TagSets { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        ITenantRepository Tenants { get; }
        ITenantVariablesRepository TenantVariables { get; }
        IUserRepository Users { get; }
        IUserPermissionsRepository UserPermissions { get; }
        IUserRolesRepository UserRoles { get; }
        IVariableSetRepository VariableSets { get; }
        IWorkerPoolRepository WorkerPools { get; }
        IWorkerRepository Workers { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        SpaceContext SpaceContext { get; }
        IOctopusRepository ForSpaceContext(string spaceId);
        IOctopusRepository ForSpaceAndSystemContext(string spaceId);
        IOctopusRepository ForSystemContext();
        SpaceRootResource SpaceRootDocument { get; }
        RootResource RootDocument { get; }
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
        RootResource RefreshRootDocument();
    }
}
#endif