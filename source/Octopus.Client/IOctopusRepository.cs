using System;
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
        IArtifactRepository Artifacts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        IBackupRepository Backups { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        ICertificateRepository Certificates { get; }
        IChannelRepository Channels { get; }
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
        IMachineRepository Machines { get; }
        IMachineRoleRepository MachineRoles { get; }
        IMachinePolicyRepository MachinePolicies { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }
        IProjectGroupRepository ProjectGroups { get; }
        IProjectRepository Projects { get; }
        IReleaseRepository Releases { get; }
        IProxyRepository Proxies { get; }
        IServerStatusRepository ServerStatus { get; }
        ISchedulerRepository Schedulers { get; }
        ISubscriptionRepository Subscriptions { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        ITagSetRepository TagSets { get; }
        ITenantRepository Tenants { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IVariableSetRepository VariableSets { get; }
        IProjectTriggerRepository ProjectTriggers { get; }
        IRetentionPolicyRepository RetentionPolicies { get; }

    }
}