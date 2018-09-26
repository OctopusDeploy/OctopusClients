#if SYNC_CLIENT
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
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
        readonly string rootDocumentUri = "~/api";
        public OctopusRepository(OctopusServerEndpoint endpoint) : this(new OctopusClient(endpoint))
        {
        }

        public OctopusRepository(IOctopusClient client, SpaceContext spaceContext = null)
        {
            Client = client;
            RootDocument = LoadRootDocument();
            var space = TryGetSpace(spaceContext);
            SpaceContext = space == null ? SpaceContext.SystemOnly() : 
                spaceContext?.IncludeSystem == true ? SpaceContext.SpecificSpaceAndSystem(space.Id) : SpaceContext.SpecificSpace(space.Id);

            SpaceRootDocument = LoadSpaceRootResource(space?.Id);
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
        public SpaceContext SpaceContext { get; }

        public IOctopusRepository ForSpaceContext(string spaceId)
        {
            ValidateSpaceId(spaceId);
            return new OctopusRepository(Client, SpaceContext.SpecificSpace(spaceId));
        }

        public IOctopusRepository ForSpaceAndSystemContext(string spaceId)
        {
            ValidateSpaceId(spaceId);
            return new OctopusRepository(Client, SpaceContext.SpecificSpaceAndSystem(spaceId));
        }

        public IOctopusRepository ForSystemContext()
        {
            return new OctopusRepository(Client, SpaceContext.SystemOnly());
        }

        public SpaceRootResource SpaceRootDocument { get; private set; }
        public RootResource RootDocument { get; private set; }

        public bool HasLink(string name)
        {
            return SpaceRootDocument != null && SpaceRootDocument.HasLink(name) || RootDocument.HasLink(name);
        }

        public string Link(string name)
        {
            return SpaceRootDocument != null && SpaceRootDocument.Links.TryGetValue(name, out var value)
                ? value.AsString()
                : RootDocument.Link(name);
        }

        public RootResource RefreshRootDocument()
        {
            RootDocument = Client.Get<RootResource>(rootDocumentUri);
            return RootDocument;
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

        SpaceRootResource LoadSpaceRootResource(string spaceId)
        {
            return !string.IsNullOrEmpty(spaceId) ?
                Client.Get<SpaceRootResource>(RootDocument.Link("SpaceHome"), new { spaceId })
                : null;
        }

        SpaceResource TryGetSpace(SpaceContext spaceContext)
        {
            if (Client.IsAuthenticated)
            {
                var currentUser =
                    Client.Get<UserResource>(RootDocument.Links["CurrentUser"]);
                var userSpaces = Client.Get<SpaceResource[]>(currentUser.Links["Spaces"]);
                // If user explicitly specified the spaceId e.g. from the command line, we might use it
                return spaceContext == null ? userSpaces.SingleOrDefault(s => s.IsDefault) :
                    spaceContext.SpaceIds.Any() ? userSpaces.Single(s => s.Id == spaceContext.SpaceIds.Single()) : null;
            }
            return null;
        }
        RootResource LoadRootDocument()
        {
            RootResource server;

            var watch = Stopwatch.StartNew();
            Exception lastError = null;

            // 60 second limit using Stopwatch alone makes debugging impossible.
            var retries = 3;

            while (true)
            {
                if (retries <= 0 && TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds) > TimeSpan.FromSeconds(60))
                {
                    if (lastError == null)
                    {
                        throw new Exception("Unable to connect to the Octopus Deploy server.");
                    }

                    throw new Exception("Unable to connect to the Octopus Deploy server. See the inner exception for details.", lastError);
                }

                try
                {
                    server = Client.Get<RootResource>(this.rootDocumentUri);
                    break;
                }
                catch (WebException ex)
                {
                    Thread.Sleep(1000);
                    lastError = ex;
                }
                catch (OctopusServerException ex)
                {
                    if (ex.HttpStatusCode != 503)
                    {
                        // 503 means the service is starting, so give it some time to start
                        throw;
                    }

                    Thread.Sleep(500);
                    lastError = ex;
                }
                retries--;
            }

            if (string.IsNullOrWhiteSpace(server.ApiVersion))
                throw new UnsupportedApiVersionException("This Octopus Deploy server uses an older API specification than this tool can handle. Please check for updates to the Octo tool.");

            var min = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMin);
            var max = SemanticVersion.Parse(ApiConstants.SupportedApiSchemaVersionMax);
            var current = SemanticVersion.Parse(server.ApiVersion);

            if (current < min || current > max)
                throw new UnsupportedApiVersionException(string.Format("This Octopus Deploy server uses a newer API specification ({0}) than this tool can handle ({1} to {2}). Please check for updates to this tool.", server.ApiVersion, ApiConstants.SupportedApiSchemaVersionMin, ApiConstants.SupportedApiSchemaVersionMax));

            return server;
        }
    }
}
#endif