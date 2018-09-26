using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public interface ISpaceScopedAsyncRepository
    {
        IAccountRepository Accounts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        IArtifactRepository Artifacts { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        ICertificateConfigurationRepository CertificateConfiguration { get; }
        ICertificateRepository Certificates { get; }
        IChannelRepository Channels { get; }
        IDashboardConfigurationRepository DashboardConfigurations { get; }
        IDashboardRepository Dashboards { get; }
        IDefectsRepository Defects { get; }
        IDeploymentProcessRepository DeploymentProcesses { get; }
        IDeploymentRepository Deployments { get; }
        IEnvironmentRepository Environments { get; }
        IFeedRepository Feeds { get; }
        IInterruptionRepository Interruptions { get; }
        ILibraryVariableSetRepository LibraryVariableSets { get; }
        ILifecyclesRepository Lifecycles { get; }
        IMachinePolicyRepository MachinePolicies { get; }
        IMachineRepository Machines { get; }
        IMachineRoleRepository MachineRoles { get; }
        IProjectGroupRepository ProjectGroups { get; }
        IProjectRepository Projects { get; }
        IProjectTriggerRepository ProjectTriggers { get; }
        IProxyRepository Proxies { get; }
        IReleaseRepository Releases { get; }
        IRetentionPolicyRepository RetentionPolicies { get; }
        ITagSetRepository TagSets { get; }
        ITenantRepository Tenants { get; }
        ITenantVariablesRepository TenantVariables { get; }
        IVariableSetRepository VariableSets { get; }
        IWorkerPoolRepository WorkerPools { get; }
        IWorkerRepository Workers { get; }
        SpaceRootResource SpaceRootDocument { get; }
    }

    public interface ISystemScopedAsyncRepository
    {
        ISchedulerRepository Schedulers { get; }
        IBackupRepository Backups { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        IConfigurationRepository Configuration { get; }
        IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        IMigrationRepository Migrations { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }
        IPerformanceConfigurationRepository PerformanceConfiguration { get; }
        IServerStatusRepository ServerStatus { get; }
        ISpaceRepository Spaces { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
    }

    public interface IMixedScopeAsyncRepository
    {
        ISubscriptionRepository Subscriptions { get; }
        IEventRepository Events { get; }
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        IScopedUserRoleRepository ScopedUserRoles { get; }
        IUserPermissionsRepository UserPermissions { get; }
    }

    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusAsyncRepository : ISpaceScopedAsyncRepository, ISystemScopedAsyncRepository, IMixedScopeAsyncRepository
    { 
        IOctopusAsyncClient Client { get; }
        SpaceContext SpaceContext { get; }
        Task<ISpaceScopedAsyncRepository> ForSpaceContext(string spaceId);
        Task<IOctopusAsyncRepository> ForSpaceAndSystemContext(string spaceId);
        ISystemScopedAsyncRepository ForSystemContext();
        bool HasLink(string name);
        string Link(string name);
    }
}