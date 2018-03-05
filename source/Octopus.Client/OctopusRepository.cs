#if SYNC_CLIENT
using Octopus.Client.Repositories;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusRepository.Client" />.
    /// </summary>
    /// <remarks>
    /// Create using:
    /// <code>
    /// var repository = new OctopusRepository(new OctopusServerEndpoint("http://myoctopus/"));
    /// </code>
    /// </remarks>
    public class OctopusRepository : IOctopusRepository
    {
        public OctopusRepository(OctopusServerEndpoint endpoint) : this(new OctopusClient(endpoint))
        {
        }

        public OctopusRepository(IOctopusClient client)
        {
            this.Client = client;

            Accounts = new AccountRepository(client);
            ActionTemplates = new ActionTemplateRepository(client);
            Artifacts = new ArtifactRepository(client);
            Backups = new BackupRepository(client);
            BuiltInPackageRepository = new BuiltInPackageRepositoryRepository(client);
            CertificateConfiguration = new CertificateConfigurationRepository(client);
            Certificates = new CertificateRepository(client);
            Channels = new ChannelRepository(client);
            CommunityActionTemplates = new CommunityActionTemplateRepository(client);
            Configuration = new ConfigurationRepository(client);
            DashboardConfigurations = new DashboardConfigurationRepository(client);
            Dashboards = new DashboardRepository(client);
            Defects = new DefectsRepository(client);
            DeploymentProcesses = new DeploymentProcessRepository(client);
            Deployments = new DeploymentRepository(client);
            Environments = new EnvironmentRepository(client);
            Events = new EventRepository(client);
            FeaturesConfiguration = new FeaturesConfigurationRepository(client);
            Feeds = new FeedRepository(client);
            Interruptions = new InterruptionRepository(client);
            LibraryVariableSets = new LibraryVariableSetRepository(client);
            Lifecycles = new LifecyclesRepository(client);
            MachinePolicies = new MachinePolicyRepository(client);
            MachineRoles = new MachineRoleRepository(client);
            Machines = new MachineRepository(client);
            Migrations = new MigrationRepository(client);
            OctopusServerNodes = new OctopusServerNodeRepository(client);
            ProjectGroups = new ProjectGroupRepository(client);
            Projects = new ProjectRepository(client);
            ProjectTriggers = new ProjectTriggerRepository(client);
            Proxies = new ProxyRepository(client);
            Releases = new ReleaseRepository(client);
            RetentionPolicies = new RetentionPolicyRepository(client);
            Schedulers = new SchedulerRepository(client);
            ServerStatus = new ServerStatusRepository(client);
            Subscriptions = new SubscriptionRepository(client);
            TagSets = new TagSetRepository(client);
            Tasks = new TaskRepository(client);
            Teams = new TeamsRepository(client);
            Tenants = new TenantRepository(client);
            UserRoles = new UserRolesRepository(client);
            Users = new UserRepository(client);
            VariableSets = new VariableSetRepository(client);
        }

        public IOctopusClient Client { get; }

        public IAccountRepository Accounts { get; }
        public IActionTemplateRepository ActionTemplates { get; }
        public IArtifactRepository Artifacts { get; }
        public IBackupRepository Backups { get; }
        public IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }
        public ICertificateConfigurationRepository CertificateConfiguration { get; }
        public ICertificateRepository Certificates { get; }
        public IChannelRepository Channels { get; }
        public ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        public IConfigurationRepository Configuration { get; }
        public IDashboardConfigurationRepository DashboardConfigurations { get; }
        public IDashboardRepository Dashboards { get; }
        public IDefectsRepository Defects { get; }
        public IDeploymentProcessRepository DeploymentProcesses { get; }
        public IDeploymentRepository Deployments { get; }
        public IEnvironmentRepository Environments { get; }
        public IEventRepository Events { get; }
        public IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        public IFeedRepository Feeds { get; }
        public IInterruptionRepository Interruptions { get; }
        public ILibraryVariableSetRepository LibraryVariableSets { get; }
        public ILifecyclesRepository Lifecycles { get; }
        public IMachinePolicyRepository MachinePolicies { get; }
        public IMachineRepository Machines { get; }
        public IMachineRoleRepository MachineRoles { get; }
        public IMigrationRepository Migrations { get; }
        public IOctopusServerNodeRepository OctopusServerNodes { get; }
        public IProjectGroupRepository ProjectGroups { get; }
        public IProjectRepository Projects { get; }
        public IProjectTriggerRepository ProjectTriggers { get; }
        public IProxyRepository Proxies { get; }
        public IReleaseRepository Releases { get; }
        public IRetentionPolicyRepository RetentionPolicies { get; }
        public ISchedulerRepository Schedulers { get; }
        public IServerStatusRepository ServerStatus { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public ITagSetRepository TagSets { get; }
        public ITaskRepository Tasks { get; }
        public ITeamsRepository Teams { get; }
        public ITenantRepository Tenants { get; }
        public IUserRepository Users { get; }
        public IUserRolesRepository UserRoles { get; }
        public IVariableSetRepository VariableSets { get; }
    }
}
#endif