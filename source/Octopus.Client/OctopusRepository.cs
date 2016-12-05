#if SYNC_CLIENT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories;
using Octopus.Client.Model.Triggers;

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
    // [Obsolete("Use IOctopusAsyncRepository instead")]
    public class OctopusRepository : IOctopusRepository
    {
        readonly IOctopusClient client;
        readonly IAccountRepository accounts;
        readonly IActionTemplateRepository actionTemplates;
        readonly ICommunityActionTemplateRepository communityActionTemplates;
        readonly IFeedRepository feeds;
        readonly IBackupRepository backups;
        readonly IMachineRepository machines;
        readonly IMachineRoleRepository machineRoles;
        readonly IMachinePolicyRepository machinePolicies;
        readonly ISubscriptionRepository subscriptions;
        readonly IEnvironmentRepository environments;
        readonly IEventRepository events;
        readonly IFeaturesConfigurationRepository featuresConfiguration;
        readonly IProjectGroupRepository projectGroups;
        readonly IProjectRepository projects;
        readonly IProxyRepository proxies;
        readonly ITaskRepository tasks;
        readonly IUserRepository users;
        readonly IVariableSetRepository variableSets;
        readonly ILibraryVariableSetRepository libraryVariableSets;
        readonly IDeploymentProcessRepository deploymentProcesses;
        readonly IReleaseRepository releases;
        readonly IDeploymentRepository deployments;
        readonly ICertificateRepository certificates;
        readonly IDashboardRepository dashboards;
        readonly IDashboardConfigurationRepository dashboardConfigurations;
        readonly IArtifactRepository artifacts;
        readonly IInterruptionRepository interruptions;
        readonly IServerStatusRepository serverStatus;
        readonly IUserRolesRepository userRoles;
        readonly ITeamsRepository teams;
        readonly IRetentionPolicyRepository retentionPolicies;
        readonly IDefectsRepository defects;
        readonly ILifecyclesRepository lifecycles;
        readonly IOctopusServerNodeRepository octopusServerNodes;
        readonly IChannelRepository channels;
        readonly IProjectTriggerRepository projectTriggers;
        readonly ISchedulerRepository schedulers;
        readonly ITenantRepository tenants;
        readonly ITagSetRepository tagSets;
        readonly IBuiltInPackageRepositoryRepository builtInPackageRepositoryRepository;

        public OctopusRepository(OctopusServerEndpoint endpoint) : this(new OctopusClient(endpoint))
        {
        }

        public OctopusRepository(IOctopusClient client)
        {
            this.client = client;
            feeds = new FeedRepository(client);
            backups = new BackupRepository(client);
            machines = new MachineRepository(client);
            machineRoles = new MachineRoleRepository(client);
            machinePolicies = new MachinePolicyRepository(client);
            subscriptions = new SubscriptionRepository(client);
            environments = new EnvironmentRepository(client);
            events = new EventRepository(client);
            featuresConfiguration = new FeaturesConfigurationRepository(client);
            projectGroups = new ProjectGroupRepository(client);
            projects = new ProjectsRepository(client);
            proxies = new ProxyRepository(client);
            tasks = new TaskRepository(client);
            users = new UserRepository(client);
            variableSets = new VariableSetRepository(client);
            libraryVariableSets = new LibraryVariableSetRepository(client);
            deploymentProcesses = new DeploymentProcessRepository(client);
            releases = new ReleaseRepository(client);
            deployments = new DeploymentRepository(client);
            certificates = new CertificateRepository(client);
            dashboards = new DashboardRepository(client);
            dashboardConfigurations = new DashboardConfigurationRepository(client);
            artifacts = new ArtifactRepository(client);
            interruptions = new InterruptionRepository(client);
            serverStatus = new ServerStatusRepository(client);
            userRoles = new UserRolesRepository(client);
            teams = new TeamsRepository(client);
            retentionPolicies = new RetentionPolicyRepository(client);
            accounts = new AccountRepository(client);
            defects = new DefectsRepository(client);
            lifecycles = new LifecyclesRepository(client);
            octopusServerNodes = new OctopusServerNodeRepository(client);
            channels = new ChannelRepository(client);
            projectTriggers = new ProjectTriggerRepository(client);
            schedulers = new SchedulerRepository(client);
            tenants = new TenantRepository(client);
            tagSets = new TagSetRepository(client);
            builtInPackageRepositoryRepository = new BuiltInPackageRepositoryRepository(client);
            actionTemplates = new ActionTemplateRepository(client);
            communityActionTemplates = new CommunityActionTemplateRepository(client);
        }

        public IOctopusClient Client
        {
            get { return client; }
        }

        public IDashboardRepository Dashboards
        {
            get { return dashboards; }
        }

        public IDashboardConfigurationRepository DashboardConfigurations
        {
            get { return dashboardConfigurations; }
        }

        public IBuiltInPackageRepositoryRepository BuiltInPackageRepository
        {
            get { return builtInPackageRepositoryRepository; }
        }

        public IFeedRepository Feeds
        {
            get { return feeds; }
        }

        public IAccountRepository Accounts
        {
            get { return accounts; }
        }

        public IActionTemplateRepository ActionTemplates
        {
            get {  return actionTemplates; }
        }

        public ICommunityActionTemplateRepository CommunityActionTemplates
        {
            get { return communityActionTemplates; }
        }

        public IBackupRepository Backups
        {
            get { return backups; }
        }

        public IMachineRepository Machines
        {
            get { return machines; }
        }

        public IMachineRoleRepository MachineRoles
        {
            get { return machineRoles; }
        }

        public IMachinePolicyRepository MachinePolicies
        {
            get { return machinePolicies; }
        }

        public ISubscriptionRepository Subscriptions
        {
            get { return subscriptions; }
        }

        public ILifecyclesRepository Lifecycles
        {
            get { return lifecycles; }
        }

        public IReleaseRepository Releases
        {
            get { return releases; }
        }

        public IDefectsRepository Defects
        {
            get { return defects; }
        }

        public IDeploymentRepository Deployments
        {
            get { return deployments; }
        }

        public IEnvironmentRepository Environments
        {
            get { return environments; }
        }

        public IEventRepository Events
        {
            get { return events; }
        }
        public IFeaturesConfigurationRepository FeaturesConfiguration
        {
            get { return featuresConfiguration; }
        }

        public IInterruptionRepository Interruptions
        {
            get { return interruptions; }
        }

        public IProjectGroupRepository ProjectGroups
        {
            get { return projectGroups; }
        }

        public IProjectRepository Projects
        {
            get { return projects; }
        }

        public IProxyRepository Proxies
        {
            get { return proxies; }
        }

        public ITaskRepository Tasks
        {
            get { return tasks; }
        }

        public IUserRepository Users
        {
            get { return users; }
        }

        public IUserRolesRepository UserRoles
        {
            get { return userRoles; }
        }

        public ITeamsRepository Teams
        {
            get { return teams; }
        }

        public IVariableSetRepository VariableSets
        {
            get { return variableSets; }
        }

        public ILibraryVariableSetRepository LibraryVariableSets
        {
            get { return libraryVariableSets; }
        }

        public IDeploymentProcessRepository DeploymentProcesses
        {
            get { return deploymentProcesses; }
        }

        public ICertificateRepository Certificates
        {
            get { return certificates; }
        }

        public IArtifactRepository Artifacts
        {
            get { return artifacts; }
        }

        public IServerStatusRepository ServerStatus
        {
            get { return serverStatus; }
        }

        public IRetentionPolicyRepository RetentionPolicies
        {
            get { return retentionPolicies; }
        }

        public IOctopusServerNodeRepository OctopusServerNodes
        {
            get { return octopusServerNodes; }
        }

        public IChannelRepository Channels
        {
            get { return channels; }
        }

        public IProjectTriggerRepository ProjectTriggers
        {
            get { return projectTriggers; }
        }
        public ISchedulerRepository Schedulers
        {
            get { return schedulers; }
        }

        public ITagSetRepository TagSets
        {
            get { return tagSets;  }
        }
        public ITenantRepository Tenants
        {
            get { return tenants; }
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable MemberCanBeProtected.Local
        abstract class BasicRepository<TResource> where TResource : class, IResource
        {
            readonly IOctopusClient client;
            protected readonly string CollectionLinkName;

            protected BasicRepository(IOctopusClient client, string collectionLinkName)
            {
                this.client = client;
                this.CollectionLinkName = collectionLinkName;
            }

            public IOctopusClient Client
            {
                get { return client; }
            }

            public TResource Create(TResource resource)
            {
                return client.Create(client.RootDocument.Link(CollectionLinkName), resource);
            }

            public TResource Modify(TResource resource)
            {
                return client.Update(resource.Links["Self"], resource);
            }

            public void Delete(TResource resource)
            {
                client.Delete(resource.Links["Self"]);
            }

            public void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
            {
                client.Paginate(path ?? client.RootDocument.Link(CollectionLinkName), pathParameters ?? new {}, getNextPage);
            }

            public TResource FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                TResource resource = null;
                Paginate(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, path, pathParameters);
                return resource;
            }

            public List<TResource> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                var resources = new List<TResource>();
                Paginate(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, path, pathParameters);
                return resources;
            }

            public List<TResource> FindAll(string path = null, object pathParameters = null)
            {
                return FindMany(r => true, path, pathParameters);
            }

            public List<TResource> GetAll()
            {
                return client.Get<List<TResource>>(client.RootDocument.Link(CollectionLinkName), new {id = "all"});
            }

            public TResource FindByName(string name, string path = null, object pathParameters = null)
            {
                name = (name ?? string.Empty).Trim();
                return FindOne(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                    return false;
                }, path, pathParameters);
            }

            public List<TResource> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null)
            {
                var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
                return FindMany(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                    return false;
                }, path, pathParameters);
            }

            public TResource Get(string idOrHref)
            {
                if (string.IsNullOrWhiteSpace(idOrHref))
                    return null;

                if (idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    return client.Get<TResource>(idOrHref);
                }

                return client.Get<TResource>(client.RootDocument.Link(CollectionLinkName), new {id = idOrHref});
            }

            public List<TResource> Get(params string[] ids)
            {
                if (ids == null) return new List<TResource>();
                var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
                if (actualIds.Length == 0) return new List<TResource>();

                var resources = new List<TResource>();
                client.Paginate<TResource>(
                    client.RootDocument.Link(CollectionLinkName) + "{?ids}",
                    new {ids = actualIds},
                    page =>
                    {
                        resources.AddRange(page.Items);
                        return true;
                    });

                return resources;
            }

            public TResource Refresh(TResource resource)
            {
                if (resource == null) throw new ArgumentNullException("resource");
                return Get(resource.Id);
            }
        }

        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBeProtected.Local
        class ServerStatusRepository : BasicRepository<ServerStatusResource>, IServerStatusRepository
        {
            public ServerStatusRepository(IOctopusClient client)
                : base(client, null) // Not a collection
            {
            }

            public ServerStatusResource GetServerStatus()
            {
                return Client.Get<ServerStatusResource>(Client.RootDocument.Link("ServerStatus"));
            }

            public SystemInfoResource GetSystemInfo(ServerStatusResource status)
            {
                if (status == null) throw new ArgumentNullException("status");
                return Client.Get<SystemInfoResource>(status.Link("SystemInfo"));
            }
        }

        class BackupRepository : IBackupRepository
        {
            readonly IOctopusClient client;

            public BackupRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public BackupConfigurationResource GetConfiguration()
            {
                return client.Get<BackupConfigurationResource>(client.RootDocument.Link("BackupConfiguration"));
            }

            public BackupConfigurationResource ModifyConfiguration(BackupConfigurationResource backupConfiguration)
            {
                return client.Update(backupConfiguration.Link("Self"), backupConfiguration);
            }
        }

        class UserRolesRepository : BasicRepository<UserRoleResource>, IUserRolesRepository
        {
            public UserRolesRepository(IOctopusClient client)
                : base(client, "UserRoles")
            {
            }
        }

        class TeamsRepository : BasicRepository<TeamResource>, ITeamsRepository
        {
            public TeamsRepository(IOctopusClient client)
                : base(client, "Teams")
            {
            }
        }

        class ArtifactRepository : BasicRepository<ArtifactResource>, IArtifactRepository
        {
            public ArtifactRepository(IOctopusClient client)
                : base(client, "Artifacts")
            {
            }

            public Stream GetContent(ArtifactResource artifact)
            {
                return Client.GetContent(artifact.Link("Content"));
            }

            public void PutContent(ArtifactResource artifact, Stream contentStream)
            {
                Client.PutContent(artifact.Link("Content"), contentStream);
            }

            public ResourceCollection<ArtifactResource> FindRegarding(IResource resource)
            {
                return Client.List<ArtifactResource>(Client.RootDocument.Link("Artifacts"), new {regarding = resource.Id});
            }
        }

        class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
        {
            public AccountRepository(IOctopusClient client)
                : base(client, "Accounts")
            {
            }
        }

        class ActionTemplateRepository : BasicRepository<ActionTemplateResource>, IActionTemplateRepository
        {
            public ActionTemplateRepository(IOctopusClient client) : base(client, "ActionTemplates")
            {
            }

            public List<ActionTemplateSearchResource> Search()
            {
                return Client.Get<List<ActionTemplateSearchResource>>(Client.RootDocument.Link("ActionTemplatesSearch"));
            }
        }

        class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
        {
            public CommunityActionTemplateRepository(IOctopusClient client) : base(client, "CommunityActionTemplates")
            {
            }
        }

        class FeedRepository : BasicRepository<FeedResource>, IFeedRepository
        {
            public FeedRepository(IOctopusClient client) : base(client, "Feeds")
            {
            }

            public List<PackageResource> GetVersions(FeedResource feed, string[] packageIds)
            {
                return Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), new {packageIds = packageIds});
            }
        }

        class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
        {
            public RetentionPolicyRepository(IOctopusClient client)
                : base(client, "RetentionPolicies")
            {
            }

            public TaskResource ApplyNow()
            {
                var tasks = new TaskRepository(Client);
                var task = new TaskResource {Name = "Retention", Description = "Request to apply retention policies via the API"};
                return tasks.Create(task);
            }
        }

        class MachineRepository : BasicRepository<MachineResource>, IMachineRepository
        {
            public MachineRepository(IOctopusClient client) : base(client, "Machines")
            {
            }

            public MachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
            {
                return Client.Get<MachineResource>(Client.RootDocument.Link("DiscoverMachine"), new {host, port, type});
            }

            public MachineConnectionStatus GetConnectionStatus(MachineResource machine)
            {
                if (machine == null) throw new ArgumentNullException("machine");
                return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
            }

            public List<MachineResource> FindByThumbprint(string thumbprint)
            {
                if (thumbprint == null) throw new ArgumentNullException("thumbprint");
                return Client.Get<List<MachineResource>>(Client.RootDocument.Link("machines"), new { id="all", thumbprint });                
            }

            public MachineEditor CreateOrModify(
                string name,
                EndpointResource endpoint,
                EnvironmentResource[] environments,
                string[] roles,
                TenantResource[] tenants,
                TagResource[] tenantTags)
            {
                return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags);
            }

            public MachineEditor CreateOrModify(
                string name,
                EndpointResource endpoint,
                EnvironmentResource[] environments,
                string[] roles)
            {
                return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles);
            }
        }

        class MachinePolicyRepository : BasicRepository<MachinePolicyResource>, IMachinePolicyRepository
        {
            public MachinePolicyRepository(IOctopusClient client) : base(client, "MachinePolicies")
            {
            }
        }

        class SubscriptionRepository : BasicRepository<SubscriptionResource>, ISubscriptionRepository
        {
            public SubscriptionRepository(IOctopusClient client) : base(client, "Subscriptions")
            {
            }

            public SubscriptionEditor CreateOrModify(string name, EventNotificationSubscription eventNotificationSubscription, bool isDisabled)
            {
                return new SubscriptionEditor(this).CreateOrModify(name, eventNotificationSubscription, isDisabled);
            }
        }

        class ProjectsRepository : BasicRepository<ProjectResource>, IProjectRepository
        {
            public ProjectsRepository(IOctopusClient client)
                : base(client, "Projects")
            {
            }

            public ResourceCollection<ReleaseResource> GetReleases(ProjectResource project, int skip = 0)
            {
                return Client.List<ReleaseResource>(project.Link("Releases"), new {skip});
            }

            public ReleaseResource GetReleaseByVersion(ProjectResource project, string version)
            {
                return Client.Get<ReleaseResource>(project.Link("Releases"), new {version});
            }

            public ResourceCollection<ChannelResource> GetChannels(ProjectResource project)
            {
                return Client.List<ChannelResource>(project.Link("Channels"));
            }

            public ResourceCollection<ProjectTriggerResource> GetTriggers(ProjectResource project)
            {
                return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
            }

            public void SetLogo(ProjectResource project, string fileName, Stream contents)
            {
                Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
            }

            public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
            {
                return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle);
            }

            public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description)
            {
                return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle, description);
            }
        }

        class ProxyRepository : BasicRepository<ProxyResource>, IProxyRepository
        {
            public ProxyRepository(IOctopusClient client)
                : base(client, "Proxies")
            {
                
            }
        }

        class LifecyclesRepository : BasicRepository<LifecycleResource>, ILifecyclesRepository
        {
            public LifecyclesRepository(IOctopusClient client)
                : base(client, "Lifecycles")
            {
            }

            public LifecycleEditor CreateOrModify(string name)
            {
                return new LifecycleEditor(this).CreateOrModify(name);
            }

            public LifecycleEditor CreateOrModify(string name, string description)
            {
                return new LifecycleEditor(this).CreateOrModify(name, description);
            }
        }

        class DefectsRepository : BasicRepository<DefectResource>, IDefectsRepository
        {
            public DefectsRepository(IOctopusClient client)
                : base(client, "Defects")
            {
            }

            public ResourceCollection<DefectResource> GetDefects(ReleaseResource release)
            {
                return Client.List<DefectResource>(release.Link("Defects"));
            }

            public void RaiseDefect(ReleaseResource release, string description)
            {
                Client.Post(release.Link("ReportDefect"), new DefectResource(description));
            }

            public void ResolveDefect(ReleaseResource release)
            {
                Client.Post(release.Link("ResolveDefect"));
            }
        }

        class ReleaseRepository : BasicRepository<ReleaseResource>, IReleaseRepository
        {
            public ReleaseRepository(IOctopusClient client)
                : base(client, "Releases")
            {
            }

            public ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0)
            {
                return Client.List<DeploymentResource>(release.Link("Deployments"), new {skip});
            }

            public ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0)
            {
                return Client.List<ArtifactResource>(release.Link("Artifacts"), new {skip});
            }

            public DeploymentTemplateResource GetTemplate(ReleaseResource release)
            {
                return Client.Get<DeploymentTemplateResource>(release.Link("DeploymentTemplate"));
            }

            public DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
            {
                return Client.Get<DeploymentPreviewResource>(promotionTarget.Link("Preview"));
            }

            public ReleaseResource SnapshotVariables(ReleaseResource release)
            {
                Client.Post(release.Link("SnapshotVariables"));
                return Get(release.Id);
            }

            public ReleaseResource Create(ReleaseResource resource, bool ignoreChannelRules = false)
            {
                return Client.Create(Client.RootDocument.Link(CollectionLinkName), resource, new { ignoreChannelRules });
            }

            public ReleaseResource Modify(ReleaseResource resource, bool ignoreChannelRules = false)
            {
                return Client.Update(resource.Links["Self"], resource, new { ignoreChannelRules });
            }
        }

        class DashboardRepository : IDashboardRepository
        {
            readonly IOctopusClient client;

            public DashboardRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public DashboardResource GetDashboard()
            {
                return client.Get<DashboardResource>(client.RootDocument.Link("Dashboard"));
            }

            public DashboardResource GetDynamicDashboard(string[] projects, string[] environments)
            {
                return client.Get<DashboardResource>(client.RootDocument.Link("DashboardDynamic"), new {projects, environments});
            }
        }

        class DashboardConfigurationRepository : IDashboardConfigurationRepository
        {
            readonly IOctopusClient client;

            public DashboardConfigurationRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public DashboardConfigurationResource GetDashboardConfiguration()
            {
                return client.Get<DashboardConfigurationResource>(client.RootDocument.Link("DashboardConfiguration"));
            }

            public DashboardConfigurationResource ModifyDashboardConfiguration(DashboardConfigurationResource resource)
            {
                return client.Update(client.RootDocument.Link("DashboardConfiguration"), resource);
            }
        }

        class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
        {
            readonly IOctopusClient client;

            public BuiltInPackageRepositoryRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false)
            {
                return client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                    client.RootDocument.Link("PackageUpload"),
                    new FileUpload() {Contents = contents, FileName = fileName},
                    new {replace = replaceExisting});
            }

            public ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip=0, int take = 30)
            {
                return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new {nuGetPackageId = packageId, take, skip});
            }

            public ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30)
            {
                return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { latest = true, take, skip });
            }

            public void DeletePackage(PackageResource package)
            {
                client.Delete(client.RootDocument.Link("Packages"), new {id = package.Id});
            }
        }

        class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
        {
            public DeploymentRepository(IOctopusClient client)
                : base(client, "Deployments")
            {
            }

            public TaskResource GetTask(DeploymentResource resource)
            {
                return Client.Get<TaskResource>(resource.Link("Task"));
            }

            public ResourceCollection<DeploymentResource> FindAll(string[] projects, string[] environments, int skip = 0)
            {
                return Client.List<DeploymentResource>(Client.RootDocument.Link("Deployments"), new {skip, projects = projects ?? new string[0], environments = environments ?? new string[0]});
            }

            public void Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
            {
                Client.Paginate(Client.RootDocument.Link("Deployments"), new {projects = projects ?? new string[0], environments = environments ?? new string[0]}, getNextPage);
            }
        }

        class FeaturesConfigurationRepository : IFeaturesConfigurationRepository
        {
            readonly IOctopusClient client;

            public FeaturesConfigurationRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public FeaturesConfigurationResource GetFeaturesConfiguration()
            {
                return client.Get<FeaturesConfigurationResource>(client.RootDocument.Link("FeaturesConfiguration"));
            }

            public FeaturesConfigurationResource ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
            {
                return client.Update(client.RootDocument.Link("FeaturesConfiguration"), resource);
            }
        }

        class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
        {
            public VariableSetRepository(IOctopusClient client)
                : base(client, "Variables")
            {
            }

            public string[] GetVariableNames(string project, string[] environments)
            {
                return Client.Get<string[]>(Client.RootDocument.Link("VariableNames"), new { project, projectEnvironmentsFilter = environments  ?? new string[0]});
            }

        }

        class LibraryVariableSetRepository : BasicRepository<LibraryVariableSetResource>, ILibraryVariableSetRepository
        {
            public LibraryVariableSetRepository(IOctopusClient client)
                : base(client, "LibraryVariables")
            {
            }

            public LibraryVariableSetEditor CreateOrModify(string name)
            {
                return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name);
            }

            public LibraryVariableSetEditor CreateOrModify(string name, string description)
            {
                return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name, description);
            }
        }

        class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
        {
            public CertificateRepository(IOctopusClient client)
                : base(client, "Certificates")
            {
            }

            public CertificateResource GetOctopusCertificate()
            {
                return Get("certificate-global");
            }
        }

        class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
        {
            public DeploymentProcessRepository(IOctopusClient client)
                : base(client, "DeploymentProcesses")
            {
            }

            public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
            {
                return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id});
            }
        }

        class MachineRoleRepository : IMachineRoleRepository
        {
            readonly IOctopusClient client;

            public MachineRoleRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public List<string> GetAllRoleNames()
            {
                return client.Get<string[]>(client.RootDocument.Link("MachineRoles")).ToList();
            }
        }

        class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
        {
            public EnvironmentRepository(IOctopusClient client)
                : base(client, "Environments")
            {
            }

            public List<MachineResource> GetMachines(EnvironmentResource environment)
            {
                var resources = new List<MachineResource>();

                Client.Paginate<MachineResource>(environment.Link("Machines"), new {}, page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                });

                return resources;
            }

            public void Sort(string[] environmentIdsInOrder)
            {
                Client.Put(Client.RootDocument.Link("EnvironmentSortOrder"), environmentIdsInOrder);
            }

            public EnvironmentEditor CreateOrModify(string name)
            {
                return new EnvironmentEditor(this).CreateOrModify(name);
            }

            public EnvironmentEditor CreateOrModify(string name, string description)
            {
                return new EnvironmentEditor(this).CreateOrModify(name, description);
            }
        }

        class EventRepository : BasicRepository<EventResource>, IEventRepository
        {
            public EventRepository(IOctopusClient client)
                : base(client, "Events")
            {
            }

            public ResourceCollection<EventResource> List(int skip = 0,
                string filterByUserId = null,
                string regardingDocumentId = null,
                bool includeInternalEvents = false)
            {
                return Client.List<EventResource>(Client.RootDocument.Link("Events"), new { skip,
                    user = filterByUserId,
                    regarding = regardingDocumentId,
                    @internal = includeInternalEvents.ToString() });
            }

            public ResourceCollection<EventResource> List(int skip = 0,
                string from = null,
                string to = null,
                string regarding = null,
                string regardingAny = null,
                bool includeInternalEvents = true,
                string user = null,
                string users = null,
                string projects = null,
                string environments = null,
                string eventGroups = null,
                string eventCategories = null,
                string tenants = null,
                string tags = null)
            {
                return Client.List<EventResource>(Client.RootDocument.Link("Events"), new {skip,
                    from = from,
                    to = to,
                    regarding = regarding,
                    regardingAny = regardingAny,
                    @internal = includeInternalEvents,
                    user = user,
                    users = users,
                    projects = projects,
                    environments = environments,
                    eventGroups = eventGroups,
                    eventCategories = eventCategories,
                    tenants = tenants,
                    tags = tags
                });
            }
        }

        class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
        {
            public InterruptionRepository(IOctopusClient client)
                : base(client, "Interruptions")
            {
            }

            public ResourceCollection<InterruptionResource> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null)
            {
                return Client.List<InterruptionResource>(Client.RootDocument.Link("Interruptions"), new {skip, pendingOnly, regarding = regardingDocumentId});
            }

            public void Submit(InterruptionResource interruption)
            {
                Client.Post(interruption.Link("Submit"), interruption.Form.Values);
            }

            public void TakeResponsibility(InterruptionResource interruption)
            {
                Client.Put(interruption.Link("Responsible"), (InterruptionResource)null);
            }

            public UserResource GetResponsibleUser(InterruptionResource interruption)
            {
                return Client.Get<UserResource>(interruption.Link("Responsible"));
            }
        }

        class ProjectGroupRepository : BasicRepository<ProjectGroupResource>, IProjectGroupRepository
        {
            public ProjectGroupRepository(IOctopusClient client)
                : base(client, "ProjectGroups")
            {
            }

            public List<ProjectResource> GetProjects(ProjectGroupResource projectGroup)
            {
                var resources = new List<ProjectResource>();

                Client.Paginate<ProjectResource>(projectGroup.Link("ProjectGroups"), new {}, page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                });

                return resources;
            }

            public ProjectGroupEditor CreateOrModify(string name)
            {
                return new ProjectGroupEditor(this).CreateOrModify(name);
            }

            public ProjectGroupEditor CreateOrModify(string name, string description)
            {
                return new ProjectGroupEditor(this).CreateOrModify(name, description);
            }
        }

        class TaskRepository : BasicRepository<TaskResource>, ITaskRepository
        {
            public TaskRepository(IOctopusClient client)
                : base(client, "Tasks")
            {
            }

            public TaskResource ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.Health.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual health check" : description;
                resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.Health.Arguments.Timeout, TimeSpan.FromMinutes(timeoutAfterMinutes)},
                    {BuiltInTasks.Health.Arguments.MachineTimeout, TimeSpan.FromMinutes(machineTimeoutAfterMinutes)},
                    {BuiltInTasks.Health.Arguments.EnvironmentId, environmentId},
                    {BuiltInTasks.Health.Arguments.MachineIds, machineIds}
                };
                return Create(resource);
            }

            public TaskResource ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.UpdateCalamari.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual Calamari update" : description;
                resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.UpdateCalamari.Arguments.MachineIds, machineIds }
                };
                return Create(resource);
            }

            public TaskResource ExecuteBackup(string description = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.Backup.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description;
                return Create(resource);
            }

            public TaskResource ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.Upgrade.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual upgrade" : description;
                resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.Upgrade.Arguments.EnvironmentId, environmentId},
                    {BuiltInTasks.Upgrade.Arguments.MachineIds, machineIds}
                };
                return Create(resource);
            }

            public TaskResource ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell")
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.AdHocScript.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Run ad-hoc PowerShell script" : description;
                resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                    {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                    {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                    {BuiltInTasks.AdHocScript.Arguments.ScriptBody, scriptBody},
                    {BuiltInTasks.AdHocScript.Arguments.Syntax, syntax}
                };
                return Create(resource);
            }

          public TaskResource ExecuteActionTemplate(ActionTemplateResource template, Dictionary<string, PropertyValueResource> properties, string[] machineIds = null,
                                                    string[] environmentIds = null, string[] targetRoles = null, string description = null)
            {
                if (string.IsNullOrEmpty(template?.Id)) throw new ArgumentException("The step template was either null, or has no ID");

                var resource = new TaskResource();
                resource.Name = BuiltInTasks.AdHocScript.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Run step template: " + template.Name : description;
                resource.Arguments = new Dictionary<string, object>
                {
                    {BuiltInTasks.AdHocScript.Arguments.EnvironmentIds, environmentIds},
                    {BuiltInTasks.AdHocScript.Arguments.TargetRoles, targetRoles},
                    {BuiltInTasks.AdHocScript.Arguments.MachineIds, machineIds},
                    {BuiltInTasks.AdHocScript.Arguments.ActionTemplateId, template.Id},
                    {BuiltInTasks.AdHocScript.Arguments.Properties, properties}
                };
                return Create(resource);
            }

            public TaskResource ExecuteCommunityActionTemplatesSynchronisation(string description = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.SyncCommunityActionTemplates.Name;
                resource.Description = description ?? "Run " + BuiltInTasks.SyncCommunityActionTemplates.Name;

                return Create(resource);
            }

            public TaskDetailsResource GetDetails(TaskResource resource)
            {
                return Client.Get<TaskDetailsResource>(resource.Link("Details"));
            }

            public string GetRawOutputLog(TaskResource resource)
            {
                return Client.Get<string>(resource.Link("Raw"));
            }

            public void Rerun(TaskResource resource)
            {
                Client.Post(resource.Link("Rerun"), (TaskResource)null);
            }

            public void Cancel(TaskResource resource)
            {
                Client.Post(resource.Link("Cancel"), (TaskResource)null);
            }

            public void WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            {
                WaitForCompletion(new[] {task}, pollIntervalSeconds, timeoutAfterMinutes, interval);
            }

            public void WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            {
                var start = Stopwatch.StartNew();
                if (tasks == null || tasks.Length == 0)
                    return;

                while (true)
                {
                    var stillRunning =
                        (from task in tasks
                            let currentStatus = Client.Get<TaskResource>(task.Link("Self"))
                            select currentStatus).ToArray();

                    if (interval != null)
                    {
                        interval(stillRunning);
                    }

                    if (stillRunning.All(t => t.IsCompleted))
                        return;

                    if (timeoutAfterMinutes > 0 && start.Elapsed.TotalMinutes > timeoutAfterMinutes)
                    {
                        throw new TimeoutException(string.Format("One or more tasks did not complete before the timeout was reached. We waited {0:n1} minutes for the tasks to complete.", start.Elapsed.TotalMinutes));
                    }

                    Thread.Sleep(pollIntervalSeconds*1000);
                }
            }
        }

        class InvitationRepository : BasicRepository<InvitationResource>, ICreate<InvitationResource>
        {
            public InvitationRepository(IOctopusClient client)
                : base(client, "Invitations")
            {
            }
        }

        class UserRepository : BasicRepository<UserResource>, IUserRepository
        {
            readonly BasicRepository<InvitationResource> invitations;

            public UserRepository(IOctopusClient client)
                : base(client, "Users")
            {
                invitations = new InvitationRepository(client);
            }

            public UserResource Register(RegisterCommand registerCommand)
            {
                Client.Post(Client.RootDocument.Link("Register"), registerCommand);
                return GetCurrent();
            }

            public void SignIn(LoginCommand loginCommand)
            {
                Client.Post(Client.RootDocument.Link("SignIn"), loginCommand);
            }

            public void SignIn(string username, string password, bool rememberMe = false)
            {
                SignIn(new LoginCommand() { Username = username, Password = password, RememberMe = rememberMe });
            }

            public void SignOut()
            {
                Client.Post(Client.RootDocument.Link("SignOut"));
            }

            public UserResource GetCurrent()
            {
                return Client.Get<UserResource>(Client.RootDocument.Link("CurrentUser"));
            }

            public UserPermissionSetResource GetPermissions(UserResource user)
            {
                if (user == null) throw new ArgumentNullException("user");
                return Client.Get<UserPermissionSetResource>(user.Link("Permissions"));
            }

            public ApiKeyResource CreateApiKey(UserResource user, string purpose = null)
            {
                if (user == null) throw new ArgumentNullException("user");
                return Client.Post<object, ApiKeyResource>(user.Link("ApiKeys"), new
                {
                    Purpose = purpose ?? "Requested by Octopus.Client"
                });
            }

            public List<ApiKeyResource> GetApiKeys(UserResource user)
            {
                if (user == null) throw new ArgumentNullException("user");
                var resources = new List<ApiKeyResource>();

                Client.Paginate<ApiKeyResource>(user.Link("ApiKeys"), page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                });

                return resources;
            }

            public void RevokeApiKey(ApiKeyResource apiKey)
            {
                Client.Delete(apiKey.Link("Self"));
            }

            public InvitationResource Invite(string addToTeamId)
            {
                if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
                return Invite(new ReferenceCollection {addToTeamId});
            }

            public InvitationResource Invite(ReferenceCollection addToTeamIds)
            {
                return invitations.Create(new InvitationResource {AddToTeamIds = addToTeamIds ?? new ReferenceCollection()});
            }
        }

        class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
        {
            public OctopusServerNodeRepository(IOctopusClient client)
                : base(client, "OctopusServerNodes")
            {
            }
        }

        class ChannelRepository : BasicRepository<ChannelResource>, IChannelRepository
        {
            public ChannelRepository(IOctopusClient client)
                : base(client, "Channels")
            {
            }

            public ChannelResource FindByName(ProjectResource project, string name)
            {
                return FindByName(name, path: project.Link("Channels"));
            }

            public ChannelEditor CreateOrModify(ProjectResource project, string name)
            {
                return new ChannelEditor(this).CreateOrModify(project, name);
            }

            public ChannelEditor CreateOrModify(ProjectResource project, string name, string description)
            {
                return new ChannelEditor(this).CreateOrModify(project, name, description);
            }
        }

        class ProjectTriggerRepository : BasicRepository<ProjectTriggerResource>, IProjectTriggerRepository
        {
            public ProjectTriggerRepository(IOctopusClient client)
                : base(client, "ProjectTriggers")
            {
            }

            public ProjectTriggerResource FindByName(ProjectResource project, string name)
            {
                return FindByName(name, path: project.Link("Triggers"));
            }

            public ProjectTriggerEditor CreateOrModify(ProjectResource project, string name, TriggerFilterResource filter, TriggerActionResource action)
            {
                return new ProjectTriggerEditor(this).CreateOrModify(project, name, filter, action);
            }
        }

        class SchedulerRepository: ISchedulerRepository
        {
            readonly IOctopusClient client;

            public SchedulerRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public void Start()
            {
                client.GetContent("~/api/scheduler/start");
            }

            public void Start(string taskName)
            {
                client.GetContent($"~/api/scheduler/start?task={taskName}");
            }

            public void Trigger(string taskName)
            {
                client.GetContent($"~/api/scheduler/trigger?task={taskName}");
            }

            public void Stop()
            {
                client.GetContent("~/api/scheduler/stop");
            }

            public void Stop(string taskName)
            {
                client.GetContent($"~/api/scheduler/stop?task={taskName}");
            }
        }

        class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
        {
            public TenantRepository(IOctopusClient client)
                : base(client, "Tenants")
            {
            }

            public TenantVariableResource GetVariables(TenantResource tenant)
            {
                return Client.Get<TenantVariableResource>(tenant.Link("Variables"));
            }

            public List<TenantResource> FindAll(string name, string[] tags)
            {
                return Client.Get<List<TenantResource>>(Client.RootDocument.Link("Tenants"), new { id="all", name, tags});
            }

            public TenantVariableResource ModifyVariables(TenantResource tenant, TenantVariableResource variables)
            {
                return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
            }

            public List<TenantsMissingVariablesResource> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
            {
                return Client.Get<List<TenantsMissingVariablesResource>>(Client.RootDocument.Link("TenantsMissingVariables"), new
                {
                    tenantId = tenantId,
                    projectId = projectId,
                    environmentId = environmentId
                });
            }

            public void SetLogo(TenantResource tenant, string fileName, Stream contents)
            {
                Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
            }

            public TenantEditor CreateOrModify(string name)
            {
                return new TenantEditor(this).CreateOrModify(name);
            }
        }

        class TagSetRepository : BasicRepository<TagSetResource>, ITagSetRepository
        {
            public TagSetRepository(IOctopusClient client) : base(client, "TagSets")
            {
            }

            public void Sort(string[] tagSetIdsInOrder)
            {
                Client.Put(Client.RootDocument.Link("TagSetSortOrder"), tagSetIdsInOrder);
            }

            public TagSetEditor CreateOrModify(string name)
            {
                return new TagSetEditor(this).CreateOrModify(name);
            }

            public TagSetEditor CreateOrModify(string name, string description)
            {
                return new TagSetEditor(this).CreateOrModify(name, description);
            }
        }
    }
}
#endif