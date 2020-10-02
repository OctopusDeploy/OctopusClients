using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to space-specific parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusCommonAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusSpaceAsyncRepository: IOctopusCommonAsyncRepository
    {
        IAccountRepository Accounts { get; }
        IActionTemplateRepository ActionTemplates { get; }
        IArtifactRepository Artifacts { get; }
        IOctopusSpaceAsyncBetaRepository Beta { get; }
        IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        IBuildInformationRepository BuildInformationRepository { get; }
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
        IPackageMetadataRepository PackageMetadataRepository { get; }
        IProjectGroupRepository ProjectGroups { get; }
        IProjectRepository Projects { get; }
        IRunbookRepository Runbooks { get; }
        IRunbookProcessRepository RunbookProcesses { get; }
        IRunbookSnapshotRepository RunbookSnapshots { get; }
        IRunbookRunRepository RunbookRuns { get; }
        IProjectTriggerRepository ProjectTriggers { get; }
        IProxyRepository Proxies { get; }
        IReleaseRepository Releases { get; }
        IRetentionPolicyRepository RetentionPolicies { get; }
        ISubscriptionRepository Subscriptions { get; }
        ITagSetRepository TagSets { get; }
        ITenantRepository Tenants { get; }
        ITenantVariablesRepository TenantVariables { get; }
        IVariableSetRepository VariableSets { get; }
        IWorkerPoolRepository WorkerPools { get; }
        IWorkerRepository Workers { get; }
        Task<SpaceRootResource> LoadSpaceRootDocument();
    }
}