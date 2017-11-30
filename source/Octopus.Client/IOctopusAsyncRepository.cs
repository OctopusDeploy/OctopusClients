using System;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusAsyncRepository
    {
        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusAsyncClient Client { get; }

        IArtifactRepository Artifacts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        ICertificateRepository Certificates { get; }
        ICertificateConfigurationRepository CertificateConfiguration { get; }
        IBackupRepository Backups { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        IDashboardConfigurationRepository DashboardConfigurations { get; }
        IDashboardRepository Dashboards { get; }
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
        IProjectGroupRepository ProjectGroups { get; }
        IProjectRepository Projects { get; }
        IReleaseRepository Releases { get; }
        IProxyRepository Proxies { get; }
        IServerStatusRepository ServerStatus { get; }
        ISchedulerRepository Schedulers { get; }
        ISubscriptionRepository Subscriptions { get; }
        IScheduledProjectTriggerRepository ScheduledProjectTriggers { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        ITagSetRepository TagSets { get; }
        ITenantRepository Tenants { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IVariableSetRepository VariableSets { get; }
        IChannelRepository Channels { get; }
        IProjectTriggerRepository ProjectTriggers { get; }
        IAccountRepository Accounts { get; }
        IRetentionPolicyRepository RetentionPolicies { get; }
        IDefectsRepository Defects { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }

        IConfigurationRepository Configuration { get; }
    }
}