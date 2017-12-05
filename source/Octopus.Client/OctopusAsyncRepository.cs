using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public static class OctopusRepositoryExtensions
    {
        public static IOctopusAsyncRepository CreateRepository(this IOctopusAsyncClient client)
        {
            return new OctopusAsyncRepository(client);
        }
    }

    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusAsyncRepository.Client" />.
    /// </summary>
    /// <remarks>
    /// Create using:
    /// <code>
    /// using(var client = new OctopusAsyncClient(new OctopusServerEndpoint("http://myoctopus/"))
    /// {
    ///     var repository = client.CreateRepository();
    /// }
    /// </code>
    /// </remarks>
    public class OctopusAsyncRepository : IOctopusAsyncRepository
    {
        public OctopusAsyncRepository(IOctopusAsyncClient client)
        {
            this.Client = client;
            Feeds = new FeedRepository(client);
            Backups = new BackupRepository(client);
            ActionTemplates = new ActionTemplateRepository(client);
            CommunityActionTemplates = new CommunityActionTemplateRepository(client);
            Machines = new MachineRepository(client);
            MachineRoles = new MachineRoleRepository(client);
            MachinePolicies = new MachinePolicyRepository(client);
            Environments = new EnvironmentRepository(client);
            Events = new EventRepository(client);
            FeaturesConfiguration = new FeaturesConfigurationRepository(client);
            ProjectGroups = new ProjectGroupRepository(client);
            Projects = new ProjectRepository(client);
            Proxies = new ProxyRepository(client);
            Tasks = new TaskRepository(client);
            Users = new UserRepository(client);
            VariableSets = new VariableSetRepository(client);
            LibraryVariableSets = new LibraryVariableSetRepository(client);
            DeploymentProcesses = new DeploymentProcessRepository(client);
            Releases = new ReleaseRepository(client);
            Deployments = new DeploymentRepository(client);
            Certificates = new CertificateRepository(client);
            CertificateConfiguration = new CertificateConfigurationRepository(client);
            Dashboards = new DashboardRepository(client);
            DashboardConfigurations = new DashboardConfigurationRepository(client);
            Artifacts = new ArtifactRepository(client);
            Interruptions = new InterruptionRepository(client);
            ServerStatus = new ServerStatusRepository(client);
            UserRoles = new UserRolesRepository(client);
            Teams = new TeamsRepository(client);
            RetentionPolicies = new RetentionPolicyRepository(client);
            Accounts = new AccountRepository(client);
            Defects = new DefectsRepository(client);
            Lifecycles = new LifecyclesRepository(client);
            OctopusServerNodes = new OctopusServerNodeRepository(client);
            Channels = new ChannelRepository(client);
            ProjectTriggers = new ProjectTriggerRepository(client);
            Schedulers = new SchedulerRepository(client);
            Subscriptions = new SubscriptionRepository(client);
            Tenants = new TenantRepository(client);
            TagSets = new TagSetRepository(client);
            BuiltInPackageRepository = new BuiltInPackageRepositoryRepository(client);
            Configuration = new ConfigurationRepository(client);
        }

        public IOctopusAsyncClient Client { get; }

        public IActionTemplateRepository ActionTemplates { get; }

        public ICommunityActionTemplateRepository CommunityActionTemplates { get; }

        public IConfigurationRepository Configuration { get; }

        public IDashboardRepository Dashboards { get; }

        public IDashboardConfigurationRepository DashboardConfigurations { get; }

        public IBuiltInPackageRepositoryRepository BuiltInPackageRepository { get; }

        public IFeedRepository Feeds { get; }

        public IAccountRepository Accounts { get; }

        public IBackupRepository Backups { get; }

        public IMachineRepository Machines { get; }

        public IMachineRoleRepository MachineRoles { get; }

        public IMachinePolicyRepository MachinePolicies { get; }

        public ILifecyclesRepository Lifecycles { get; }

        public IReleaseRepository Releases { get; }

        public IDefectsRepository Defects { get; }

        public IDeploymentRepository Deployments { get; }

        public IEnvironmentRepository Environments { get; }

        public IEventRepository Events { get; }

        public IFeaturesConfigurationRepository FeaturesConfiguration { get; }

        public IInterruptionRepository Interruptions { get; }

        public IProjectGroupRepository ProjectGroups { get; }

        public IProjectRepository Projects { get; }

        public IProxyRepository Proxies { get; }

        public ISubscriptionRepository Subscriptions { get; }

        public ITaskRepository Tasks { get; }

        public IUserRepository Users { get; }

        public IUserRolesRepository UserRoles { get; }

        public ITeamsRepository Teams { get; }

        public IVariableSetRepository VariableSets { get; }

        public ILibraryVariableSetRepository LibraryVariableSets { get; }

        public IDeploymentProcessRepository DeploymentProcesses { get; }

        public ICertificateRepository Certificates { get; }

        public ICertificateConfigurationRepository CertificateConfiguration { get; }

        public IArtifactRepository Artifacts { get; }

        public IServerStatusRepository ServerStatus { get; }

        public IRetentionPolicyRepository RetentionPolicies { get; }

        public IOctopusServerNodeRepository OctopusServerNodes { get; }

        public IChannelRepository Channels { get; }

        public IProjectTriggerRepository ProjectTriggers { get; }

        public ISchedulerRepository Schedulers { get; }

        public ITagSetRepository TagSets { get; }

        public ITenantRepository Tenants { get; }
    }
}