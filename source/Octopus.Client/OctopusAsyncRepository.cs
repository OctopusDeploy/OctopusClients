using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public static class OctopusRepositoryExtensions
    {
        public static Task<IOctopusAsyncRepository> CreateRepository(this IOctopusAsyncClient client)
        {
            return OctopusAsyncRepository.Create(client);
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
        OctopusAsyncRepository(IOctopusAsyncClient client, SpaceContext spaceContext = null)
        {
            Client = client;
            SpaceContext = spaceContext;
            Accounts = new AccountRepository(this);
            ActionTemplates = new ActionTemplateRepository(this);
            Artifacts = new ArtifactRepository(this);
            Backups = new BackupRepository(this);
            BuiltInPackageRepository = new BuiltInPackageRepositoryRepository(this);
            CertificateConfiguration = new CertificateConfigurationRepository(this);
            Certificates = new CertificateRepository(this);
            Channels = new ChannelRepository(this);
            CommunityActionTemplates = new CommunityActionTemplateRepository(this);
            Configuration = new ConfigurationRepository(this);
            DashboardConfigurations = new DashboardConfigurationRepository(this);
            Dashboards = new DashboardRepository(this);
            Defects = new DefectsRepository(this);
            DeploymentProcesses = new DeploymentProcessRepository(this);
            Deployments = new DeploymentRepository(this);
            Environments = new EnvironmentRepository(this);
            Events = new EventRepository(this);
            FeaturesConfiguration = new FeaturesConfigurationRepository(this);
            Feeds = new FeedRepository(this);
            Interruptions = new InterruptionRepository(this);
            LibraryVariableSets = new LibraryVariableSetRepository(this);
            Lifecycles = new LifecyclesRepository(this);
            MachinePolicies = new MachinePolicyRepository(this);
            MachineRoles = new MachineRoleRepository(this);
            Machines = new MachineRepository(this);
            Migrations = new MigrationRepository(this);
            OctopusServerNodes = new OctopusServerNodeRepository(this);
            PerformanceConfiguration = new PerformanceConfigurationRepository(this);
            ProjectGroups = new ProjectGroupRepository(this);
            Projects = new ProjectRepository(this);
            ProjectTriggers = new ProjectTriggerRepository(this);
            Proxies = new ProxyRepository(this);
            Releases = new ReleaseRepository(this);
            RetentionPolicies = new RetentionPolicyRepository(this);
            Schedulers = new SchedulerRepository(this);
            ServerStatus = new ServerStatusRepository(this);
            Spaces = new SpaceRepository(this);
            Subscriptions = new SubscriptionRepository(this);
            TagSets = new TagSetRepository(this);
            Tasks = new TaskRepository(this);
            Teams = new TeamsRepository(this);
            Tenants = new TenantRepository(this);
            TenantVariables = new TenantVariablesRepository(this);
            UserRoles = new UserRolesRepository(this);
            Users = new UserRepository(this);
            VariableSets = new VariableSetRepository(this);
            Workers = new WorkerRepository(this);
            WorkerPools = new WorkerPoolRepository(this);
            ScopedUserRoles = new ScopedUserRoleRepository(this);
            UserPermissions = new UserPermissionsRepository(this);
        }

        public static async Task<IOctopusAsyncRepository> Create(IOctopusAsyncClient client, SpaceContext spaceContext = null)
        {
            var space = await TryGetSpace(client, spaceContext);
            spaceContext = space == null ? SpaceContext.SystemOnly() :
                spaceContext?.IncludeSystem == true ? SpaceContext.SpecificSpaceAndSystem(space.Id) : SpaceContext.SpecificSpace(space.Id);

            var spaceRoot = await LoadSpaceRootResource(client, space?.Id);
            return new OctopusAsyncRepository(client, spaceContext)
            {
                SpaceRootDocument = spaceRoot
            };
        }

        public IOctopusAsyncClient Client { get; }
        public SpaceContext SpaceContext { get; }

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
        public IPerformanceConfigurationRepository PerformanceConfiguration { get; }
        public IProjectGroupRepository ProjectGroups { get; }
        public IProjectRepository Projects { get; }
        public IProjectTriggerRepository ProjectTriggers { get; }
        public IProxyRepository Proxies { get; }
        public IReleaseRepository Releases { get; }
        public IRetentionPolicyRepository RetentionPolicies { get; }
        public ISchedulerRepository Schedulers { get; }
        public IServerStatusRepository ServerStatus { get; }
        public ISpaceRepository Spaces { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public ITagSetRepository TagSets { get; }
        public ITaskRepository Tasks { get; }
        public ITeamsRepository Teams { get; }
        public ITenantRepository Tenants { get; }
        public ITenantVariablesRepository TenantVariables { get; }
        public IUserRepository Users { get; }
        public IUserRolesRepository UserRoles { get; }
        public IVariableSetRepository VariableSets { get; }
        public IWorkerPoolRepository WorkerPools { get; }
        public IWorkerRepository Workers { get; }
        public IScopedUserRoleRepository ScopedUserRoles { get; }
        public IUserPermissionsRepository UserPermissions { get; }

        public async Task<IOctopusAsyncRepository> ForSpaceContext(string spaceId)
        {
            ValidateSpaceId(spaceId);
            var spaceRoot = await LoadSpaceRootResource(Client, spaceId);
            return new OctopusAsyncRepository(Client, SpaceContext.SpecificSpace(spaceId))
            {
                SpaceRootDocument = spaceRoot
            };
        }

        public async Task<IOctopusAsyncRepository> ForSpaceAndSystemContext(string spaceId)
        {
            ValidateSpaceId(spaceId);
            var spaceRoot = await LoadSpaceRootResource(Client, spaceId);
            return new OctopusAsyncRepository(Client, SpaceContext.SpecificSpaceAndSystem(spaceId))
            {
                SpaceRootDocument = spaceRoot
            };
        }

        public IOctopusAsyncRepository ForSystemContext()
        {
            return new OctopusAsyncRepository(Client, SpaceContext.SystemOnly());
        }

        public SpaceRootResource SpaceRootDocument { get; private set; }

        public bool HasLink(string name)
        {
            return SpaceRootDocument != null && SpaceRootDocument.HasLink(name) || Client.RootDocument.HasLink(name);
        }

        public string Link(string name)
        {
            return SpaceRootDocument != null && SpaceRootDocument.Links.TryGetValue(name, out var value)
                ? value.AsString()
                : Client.RootDocument.Link(name);
        }

        void ValidateSpaceId(string spaceId)
        {
            if (string.IsNullOrEmpty(spaceId))
            {
                throw new ArgumentException("spaceId cannot be null");
            }

            if (spaceId == MixedScopeConstants.AllSpacesQueryStringParameterValue)
            {
                throw new ArgumentException("Invalid spaceId");
            }
        }

        static async Task<SpaceRootResource> LoadSpaceRootResource(IOctopusAsyncClient client, string spaceId)
        {
            return !string.IsNullOrEmpty(spaceId) ?
                await client.Get<SpaceRootResource>(client.RootDocument.Link("SpaceHome"), new { spaceId }).ConfigureAwait(false)
                : null;
        }

        static async Task<SpaceResource> TryGetSpace(IOctopusAsyncClient client, SpaceContext spaceContext)
        {
            if (client.IsAuthenticated)
            {
                var currentUser =
                    await client.Get<UserResource>(client.RootDocument.Links["CurrentUser"]);
                var userSpaces = await client.Get<SpaceResource[]>(currentUser.Links["Spaces"]);
                return spaceContext == null ? userSpaces.SingleOrDefault(s => s.IsDefault) :
                    spaceContext.SpaceIds.Any() ? userSpaces.Single(s => s.Id == spaceContext.SpaceIds.Single()) : null;
            }

            return null;
        }
    }
}