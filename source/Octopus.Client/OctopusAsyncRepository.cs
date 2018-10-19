using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
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
        private static readonly string rootDocumentUri = "~/api";
        public OctopusAsyncRepository(IOctopusAsyncClient client, SpaceContext spaceContext = null)
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

        public IOctopusAsyncClient Client { get; }
        public SpaceContext SpaceContext { get; private set; }
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

        public SpaceRootResource SpaceRootDocument { get; private set; }
        public RootResource RootDocument { get; private set; }

        public async Task<bool> HasLink(string name)
        {
            await LoadRootDocuments();
            return SpaceRootDocument != null && SpaceRootDocument.HasLink(name) || RootDocument.HasLink(name);
        }

        public async Task<string> Link(string name)
        {
            await LoadRootDocuments();
            return SpaceRootDocument != null && SpaceRootDocument.Links.TryGetValue(name, out var value)
                ? value.AsString()
                : RootDocument.Link(name);
        }

        public async Task<RootResource> LoadRootDocument()
        {
            if (RootDocument != null)
                return RootDocument;

            RootResource server;

            var watch = Stopwatch.StartNew();
            Exception lastError = null;

            // 60 second limit using Stopwatch alone makes debugging impossible.
            var retries = 3;

            while (true)
            {
                if (retries <= 0 && watch.Elapsed > TimeSpan.FromSeconds(60))
                {
                    if (lastError == null)
                    {
                        throw new Exception("Unable to connect to the Octopus Deploy server.");
                    }

                    throw new Exception("Unable to connect to the Octopus Deploy server. See the inner exception for details.", lastError);
                }

                try
                {
                    server = await Client.Get<RootResource>(rootDocumentUri).ConfigureAwait(false);
                    break;
                }
                catch (HttpRequestException ex)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    lastError = ex;
                }
                catch (OctopusServerException ex)
                {
                    if (ex.HttpStatusCode != 503)
                    {
                        // 503 means the service is starting, so give it some time to start
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(0.5));
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
                throw new UnsupportedApiVersionException($"This Octopus Deploy server uses a newer API specification ({server.ApiVersion}) than this tool can handle ({ApiConstants.SupportedApiSchemaVersionMin} to {ApiConstants.SupportedApiSchemaVersionMax}). Please check for updates to this tool.");
            RootDocument = server;
            return server;
        }

        public async Task<SpaceRootResource> LoadSpaceRootDocument()
        {
            if (SpaceRootDocument != null)
                return SpaceRootDocument;
            if (SpaceContext == null)
            {
                var defaultSpace = await TryGetDefaultSpace();
                SpaceContext = defaultSpace == null ? SpaceContext.SystemOnly() : SpaceContext.SpecificSpaceAndSystem(defaultSpace.Id);
            }

            return SpaceContext.SpaceIds.Any() ?
                await Client.Get<SpaceRootResource>(RootDocument.Link("SpaceHome"), new { spaceId = SpaceContext.SpaceIds.Single() }).ConfigureAwait(false)
                : null;
        }

        async Task<SpaceResource> TryGetDefaultSpace()
        {
            try
            {
                var currentUser = await Client.Get<UserResource>(RootDocument.Links["CurrentUser"]);
                var userSpaces = await Client.Get<SpaceResource[]>(currentUser.Links["Spaces"]);
                return userSpaces.SingleOrDefault(s => s.IsDefault);
            }
            catch (OctopusSecurityException)
            {
                return null;
            }
        }

        async Task LoadRootDocuments()
        {
            if (RootDocument == null)
                RootDocument = await LoadRootDocument();
            if (SpaceRootDocument == null)
                SpaceRootDocument = await LoadSpaceRootDocument();
        }
    }
}