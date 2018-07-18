using System;
using Octopus.Client.Repositories.Async;
using IAccountRepository = Octopus.Client.Repositories.Async.IAccountRepository;
using IActionTemplateRepository = Octopus.Client.Repositories.Async.IActionTemplateRepository;
using IArtifactRepository = Octopus.Client.Repositories.Async.IArtifactRepository;
using IBackupRepository = Octopus.Client.Repositories.Async.IBackupRepository;
using IBuiltInPackageRepositoryRepository = Octopus.Client.Repositories.Async.IBuiltInPackageRepositoryRepository;
using ICertificateConfigurationRepository = Octopus.Client.Repositories.Async.ICertificateConfigurationRepository;
using ICertificateRepository = Octopus.Client.Repositories.Async.ICertificateRepository;
using IChannelRepository = Octopus.Client.Repositories.Async.IChannelRepository;
using ICommunityActionTemplateRepository = Octopus.Client.Repositories.Async.ICommunityActionTemplateRepository;
using IConfigurationRepository = Octopus.Client.Repositories.Async.IConfigurationRepository;
using IDashboardConfigurationRepository = Octopus.Client.Repositories.Async.IDashboardConfigurationRepository;
using IDashboardRepository = Octopus.Client.Repositories.Async.IDashboardRepository;
using IDefectsRepository = Octopus.Client.Repositories.Async.IDefectsRepository;
using IDeploymentProcessRepository = Octopus.Client.Repositories.Async.IDeploymentProcessRepository;
using IDeploymentRepository = Octopus.Client.Repositories.Async.IDeploymentRepository;
using IEnvironmentRepository = Octopus.Client.Repositories.Async.IEnvironmentRepository;
using IEventRepository = Octopus.Client.Repositories.Async.IEventRepository;
using IFeaturesConfigurationRepository = Octopus.Client.Repositories.Async.IFeaturesConfigurationRepository;
using IFeedRepository = Octopus.Client.Repositories.Async.IFeedRepository;
using IInterruptionRepository = Octopus.Client.Repositories.Async.IInterruptionRepository;
using ILibraryVariableSetRepository = Octopus.Client.Repositories.Async.ILibraryVariableSetRepository;
using ILifecyclesRepository = Octopus.Client.Repositories.Async.ILifecyclesRepository;
using IMachinePolicyRepository = Octopus.Client.Repositories.Async.IMachinePolicyRepository;
using IMachineRepository = Octopus.Client.Repositories.Async.IMachineRepository;
using IMachineRoleRepository = Octopus.Client.Repositories.Async.IMachineRoleRepository;
using IMigrationRepository = Octopus.Client.Repositories.Async.IMigrationRepository;
using IOctopusServerNodeRepository = Octopus.Client.Repositories.Async.IOctopusServerNodeRepository;
using IPerformanceConfigurationRepository = Octopus.Client.Repositories.Async.IPerformanceConfigurationRepository;
using IProjectRepository = Octopus.Client.Repositories.Async.IProjectRepository;
using IProjectTriggerRepository = Octopus.Client.Repositories.Async.IProjectTriggerRepository;
using IProxyRepository = Octopus.Client.Repositories.Async.IProxyRepository;
using IReleaseRepository = Octopus.Client.Repositories.Async.IReleaseRepository;
using IRetentionPolicyRepository = Octopus.Client.Repositories.Async.IRetentionPolicyRepository;
using ISchedulerRepository = Octopus.Client.Repositories.Async.ISchedulerRepository;
using IServerStatusRepository = Octopus.Client.Repositories.Async.IServerStatusRepository;
using ISubscriptionRepository = Octopus.Client.Repositories.Async.ISubscriptionRepository;
using ITagSetRepository = Octopus.Client.Repositories.Async.ITagSetRepository;
using ITaskRepository = Octopus.Client.Repositories.Async.ITaskRepository;
using ITeamsRepository = Octopus.Client.Repositories.Async.ITeamsRepository;
using ITenantRepository = Octopus.Client.Repositories.Async.ITenantRepository;
using ITenantVariablesRepository = Octopus.Client.Repositories.Async.ITenantVariablesRepository;
using IUserRepository = Octopus.Client.Repositories.Async.IUserRepository;
using IUserRolesRepository = Octopus.Client.Repositories.Async.IUserRolesRepository;
using IVariableSetRepository = Octopus.Client.Repositories.Async.IVariableSetRepository;
using IWorkerPoolRepository = Octopus.Client.Repositories.Async.IWorkerPoolRepository;
using IWorkerRepository = Octopus.Client.Repositories.Async.IWorkerRepository;

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

        IAccountRepository Accounts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        IArtifactRepository Artifacts { get; }
        IBackupRepository Backups { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        ICertificateConfigurationRepository CertificateConfiguration { get; }
        ICertificateRepository Certificates { get; }
        IChannelRepository Channels { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
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
        ISubscriptionRepository Subscriptions { get; }
        ITagSetRepository TagSets { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        ITenantRepository Tenants { get; }
        ITenantVariablesRepository TenantVariables { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
        IVariableSetRepository VariableSets { get; }
        IWorkerPoolRepository WorkerPools { get; }
        IWorkerRepository Workers { get; }
    }
}