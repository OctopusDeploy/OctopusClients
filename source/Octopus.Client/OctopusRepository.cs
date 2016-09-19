using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Endpoints;
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
        public OctopusRepository(IOctopusClient client)
        {
            this.Client = client;
            Feeds = new FeedRepository(client);
            Backups = new BackupRepository(client);
            Machines = new MachineRepository(client);
            MachineRoles = new MachineRoleRepository(client);
            MachinePolicies = new MachinePolicyRepository(client);
            Environments = new EnvironmentRepository(client);
            Events = new EventRepository(client);
            FeaturesConfiguration = new FeaturesConfigurationRepository(client);
            ProjectGroups = new ProjectGroupRepository(client);
            Projects = new ProjectsRepository(client);
            Proxies = new ProxyRepository(client);
            Tasks = new TaskRepository(client);
            Users = new UserRepository(client);
            VariableSets = new VariableSetRepository(client);
            LibraryVariableSets = new LibraryVariableSetRepository(client);
            DeploymentProcesses = new DeploymentProcessRepository(client);
            Releases = new ReleaseRepository(client);
            Deployments = new DeploymentRepository(client);
            Certificates = new CertificateRepository(client);
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
            Tenants = new TenantRepository(client);
            TagSets = new TagSetRepository(client);
            BuiltInPackageRepository = new BuiltInPackageRepositoryRepository(client);
        }

        public IOctopusClient Client { get; }

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

        public ITaskRepository Tasks { get; }

        public IUserRepository Users { get; }

        public IUserRolesRepository UserRoles { get; }

        public ITeamsRepository Teams { get; }

        public IVariableSetRepository VariableSets { get; }

        public ILibraryVariableSetRepository LibraryVariableSets { get; }

        public IDeploymentProcessRepository DeploymentProcesses { get; }

        public ICertificateRepository Certificates { get; }

        public IArtifactRepository Artifacts { get; }

        public IServerStatusRepository ServerStatus { get; }

        public IRetentionPolicyRepository RetentionPolicies { get; }

        public IOctopusServerNodeRepository OctopusServerNodes { get; }

        public IChannelRepository Channels { get; }

        public IProjectTriggerRepository ProjectTriggers { get; }

        public ISchedulerRepository Schedulers { get; }

        public ITagSetRepository TagSets { get; }

        public ITenantRepository Tenants { get; }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable MemberCanBeProtected.Local
        abstract class BasicRepository<TResource> where TResource : class, IResource
        {
            protected readonly string CollectionLinkName;

            protected BasicRepository(IOctopusClient client, string collectionLinkName)
            {
                this.Client = client;
                this.CollectionLinkName = collectionLinkName;
            }

            public IOctopusClient Client { get; }

            public Task<TResource> Create(TResource resource)
            {
                return Client.Create(Client.RootDocument.Link(CollectionLinkName), resource);
            }

            public Task<TResource> Modify(TResource resource)
            {
                return Client.Update(resource.Links["Self"], resource);
            }

            public Task Delete(TResource resource)
            {
                return Client.Delete(resource.Links["Self"]);
            }

            public Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
            {
                return Client.Paginate(path ?? Client.RootDocument.Link(CollectionLinkName), pathParameters ?? new { }, getNextPage);
            }

            public async Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                TResource resource = null;
                await Paginate(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, path, pathParameters)
                    .ConfigureAwait(false);
                return resource;
            }

            public async Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                var resources = new List<TResource>();
                await Paginate(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, path, pathParameters)
                    .ConfigureAwait(false);
                return resources;
            }

            public Task<List<TResource>> FindAll(string path = null, object pathParameters = null)
            {
                return FindMany(r => true, path, pathParameters);
            }

            public Task<List<TResource>> GetAll()
            {
                return Client.Get<List<TResource>>(Client.RootDocument.Link(CollectionLinkName), new { id = "all" });
            }

            public Task<TResource> FindByName(string name, string path = null, object pathParameters = null)
            {
                name = (name ?? string.Empty).Trim();
                return FindOne(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                    return false;
                }, path, pathParameters);
            }

            public Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null)
            {
                var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
                return FindMany(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                    return false;
                }, path, pathParameters);
            }

            public Task<TResource> Get(string idOrHref)
            {
                if (string.IsNullOrWhiteSpace(idOrHref))
                    return null;

                return idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase) 
                    ? Client.Get<TResource>(idOrHref) 
                    : Client.Get<TResource>(Client.RootDocument.Link(CollectionLinkName), new { id = idOrHref });
            }

            public async Task<List<TResource>> Get(params string[] ids)
            {
                if (ids == null) return new List<TResource>();
                var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
                if (actualIds.Length == 0) return new List<TResource>();

                var resources = new List<TResource>();
                await Client.Paginate<TResource>(
                    Client.RootDocument.Link(CollectionLinkName) + "{?ids}",
                    new { ids = actualIds },
                    page =>
                    {
                        resources.AddRange(page.Items);
                        return true;
                    })
                    .ConfigureAwait(false);

                return resources;
            }

            public Task<TResource> Refresh(TResource resource)
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

            public Task<ServerStatusResource> GetServerStatus()
            {
                return Client.Get<ServerStatusResource>(Client.RootDocument.Link("ServerStatus"));
            }

            public Task<SystemInfoResource> GetSystemInfo(ServerStatusResource status)
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

            public Task<BackupConfigurationResource> GetConfiguration()
            {
                return client.Get<BackupConfigurationResource>(client.RootDocument.Link("BackupConfiguration"));
            }

            public Task<BackupConfigurationResource> ModifyConfiguration(BackupConfigurationResource backupConfiguration)
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

            public Task<Stream> GetContent(ArtifactResource artifact)
            {
                return Client.GetContent(artifact.Link("Content"));
            }

            public Task PutContent(ArtifactResource artifact, Stream contentStream)
            {
                return Client.PutContent(artifact.Link("Content"), contentStream);
            }

            public Task<ResourceCollection<ArtifactResource>> FindRegarding(IResource resource)
            {
                return Client.List<ArtifactResource>(Client.RootDocument.Link("Artifacts"), new { regarding = resource.Id });
            }
        }

        class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
        {
            public AccountRepository(IOctopusClient client)
                : base(client, "Accounts")
            {
            }
        }

        class FeedRepository : BasicRepository<FeedResource>, IFeedRepository
        {
            public FeedRepository(IOctopusClient client) : base(client, "Feeds")
            {
            }

            public Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds)
            {
                return Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), new { packageIds = packageIds });
            }
        }

        class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
        {
            public RetentionPolicyRepository(IOctopusClient client)
                : base(client, "RetentionPolicies")
            {
            }

            public Task<TaskResource> ApplyNow()
            {
                var tasks = new TaskRepository(Client);
                var task = new TaskResource { Name = "Retention", Description = "Request to apply retention policies via the API" };
                return tasks.Create(task);
            }
        }

        class MachineRepository : BasicRepository<MachineResource>, IMachineRepository
        {
            public MachineRepository(IOctopusClient client) : base(client, "Machines")
            {
            }

            public Task<MachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
            {
                return Client.Get<MachineResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
            }

            public Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine)
            {
                if (machine == null) throw new ArgumentNullException("machine");
                return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
            }

            public Task<List<MachineResource>> FindByThumbprint(string thumbprint)
            {
                if (thumbprint == null) throw new ArgumentNullException("thumbprint");
                return Client.Get<List<MachineResource>>(Client.RootDocument.Link("machines"), new { id = "all", thumbprint });
            }

            public Task<MachineEditor> CreateOrModify(
                string name,
                EndpointResource endpoint,
                EnvironmentResource[] environments,
                string[] roles,
                TenantResource[] tenants,
                TagResource[] tenantTags)
            {
                return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags);
            }

            public Task<MachineEditor> CreateOrModify(
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

        class ProjectsRepository : BasicRepository<ProjectResource>, IProjectRepository
        {
            public ProjectsRepository(IOctopusClient client)
                : base(client, "Projects")
            {
            }

            public Task<ResourceCollection<ReleaseResource>> GetReleases(ProjectResource project, int skip = 0)
            {
                return Client.List<ReleaseResource>(project.Link("Releases"), new { skip });
            }

            public Task<ReleaseResource> GetReleaseByVersion(ProjectResource project, string version)
            {
                return Client.Get<ReleaseResource>(project.Link("Releases"), new { version });
            }

            public Task<ResourceCollection<ChannelResource>> GetChannels(ProjectResource project)
            {
                return Client.List<ChannelResource>(project.Link("Channels"));
            }

            public Task<ResourceCollection<ProjectTriggerResource>> GetTriggers(ProjectResource project)
            {
                return Client.List<ProjectTriggerResource>(project.Link("Triggers"));
            }

            public Task SetLogo(ProjectResource project, string fileName, Stream contents)
            {
                return Client.Post(project.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
            }

            public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
            {
                return new ProjectEditor(this, new ChannelRepository(Client), new DeploymentProcessRepository(Client), new ProjectTriggerRepository(Client), new VariableSetRepository(Client)).CreateOrModify(name, projectGroup, lifecycle);
            }

            public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description)
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

            public Task<LifecycleEditor> CreateOrModify(string name)
            {
                return new LifecycleEditor(this).CreateOrModify(name);
            }

            public Task<LifecycleEditor> CreateOrModify(string name, string description)
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

            public Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release)
            {
                return Client.List<DefectResource>(release.Link("Defects"));
            }

            public Task RaiseDefect(ReleaseResource release, string description)
            {
                return Client.Post(release.Link("ReportDefect"), new DefectResource(description));
            }

            public Task ResolveDefect(ReleaseResource release)
            {
                return Client.Post(release.Link("ResolveDefect"));
            }
        }

        class ReleaseRepository : BasicRepository<ReleaseResource>, IReleaseRepository
        {
            public ReleaseRepository(IOctopusClient client)
                : base(client, "Releases")
            {
            }

            public Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, int skip = 0)
            {
                return Client.List<DeploymentResource>(release.Link("Deployments"), new { skip });
            }

            public Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, int skip = 0)
            {
                return Client.List<ArtifactResource>(release.Link("Artifacts"), new { skip });
            }

            public Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release)
            {
                return Client.Get<DeploymentTemplateResource>(release.Link("DeploymentTemplate"));
            }

            public Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
            {
                return Client.Get<DeploymentPreviewResource>(promotionTarget.Link("Preview"));
            }

            public Task<ReleaseResource> SnapshotVariables(ReleaseResource release)
            {
                Client.Post(release.Link("SnapshotVariables"));
                return Get(release.Id);
            }

            public Task<ReleaseResource> Create(ReleaseResource resource, bool ignoreChannelRules = false)
            {
                return Client.Create(Client.RootDocument.Link(CollectionLinkName), resource, new { ignoreChannelRules });
            }

            public Task<ReleaseResource> Modify(ReleaseResource resource, bool ignoreChannelRules = false)
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

            public Task<DashboardResource> GetDashboard()
            {
                return client.Get<DashboardResource>(client.RootDocument.Link("Dashboard"));
            }

            public Task<DashboardResource> GetDynamicDashboard(string[] projects, string[] environments)
            {
                return client.Get<DashboardResource>(client.RootDocument.Link("DashboardDynamic"), new { projects, environments });
            }
        }

        class DashboardConfigurationRepository : IDashboardConfigurationRepository
        {
            readonly IOctopusClient client;

            public DashboardConfigurationRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public Task<DashboardConfigurationResource> GetDashboardConfiguration()
            {
                return client.Get<DashboardConfigurationResource>(client.RootDocument.Link("DashboardConfiguration"));
            }

            public Task<DashboardConfigurationResource> ModifyDashboardConfiguration(DashboardConfigurationResource resource)
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

            public Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false)
            {
                return client.Post<FileUpload, PackageFromBuiltInFeedResource>(
                    client.RootDocument.Link("PackageUpload"),
                    new FileUpload() { Contents = contents, FileName = fileName },
                    new { replace = replaceExisting });
            }

            public Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30)
            {
                return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
            }

            public Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30)
            {
                return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { latest = true, take, skip });
            }

            public Task DeletePackage(PackageResource package)
            {
                return client.Delete(client.RootDocument.Link("Packages"), new { id = package.Id });
            }
        }

        class DeploymentRepository : BasicRepository<DeploymentResource>, IDeploymentRepository
        {
            public DeploymentRepository(IOctopusClient client)
                : base(client, "Deployments")
            {
            }

            public Task<TaskResource> GetTask(DeploymentResource resource)
            {
                return Client.Get<TaskResource>(resource.Link("Task"));
            }

            public Task<ResourceCollection<DeploymentResource>> FindAll(string[] projects, string[] environments, int skip = 0)
            {
                return Client.List<DeploymentResource>(Client.RootDocument.Link("Deployments"), new { skip, projects = projects ?? new string[0], environments = environments ?? new string[0] });
            }

            public Task Paginate(string[] projects, string[] environments, Func<ResourceCollection<DeploymentResource>, bool> getNextPage)
            {
                return Client.Paginate(Client.RootDocument.Link("Deployments"), new { projects = projects ?? new string[0], environments = environments ?? new string[0] }, getNextPage);
            }
        }

        class FeaturesConfigurationRepository : IFeaturesConfigurationRepository
        {
            readonly IOctopusClient client;

            public FeaturesConfigurationRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public Task<FeaturesConfigurationResource> GetFeaturesConfiguration()
            {
                return client.Get<FeaturesConfigurationResource>(client.RootDocument.Link("FeaturesConfiguration"));
            }

            public Task<FeaturesConfigurationResource> ModifyFeaturesConfiguration(FeaturesConfigurationResource resource)
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

            public Task<string[]> GetVariableNames(string project, string[] environments)
            {
                return Client.Get<string[]>(Client.RootDocument.Link("VariableNames"), new { project, projectEnvironmentsFilter = environments ?? new string[0] });
            }

        }

        class LibraryVariableSetRepository : BasicRepository<LibraryVariableSetResource>, ILibraryVariableSetRepository
        {
            public LibraryVariableSetRepository(IOctopusClient client)
                : base(client, "LibraryVariables")
            {
            }

            public Task<LibraryVariableSetEditor> CreateOrModify(string name)
            {
                return new LibraryVariableSetEditor(this, new VariableSetRepository(Client)).CreateOrModify(name);
            }

            public Task<LibraryVariableSetEditor> CreateOrModify(string name, string description)
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

            public Task<CertificateResource> GetOctopusCertificate()
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

            public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
            {
                return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
            }
        }

        class MachineRoleRepository : IMachineRoleRepository
        {
            readonly IOctopusClient client;

            public MachineRoleRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public async Task<IReadOnlyList<string>> GetAllRoleNames()
            {
                return await client.Get<string[]>(client.RootDocument.Link("MachineRoles")).ConfigureAwait(false);
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

                Client.Paginate<MachineResource>(environment.Link("Machines"), new { }, page =>
                 {
                     resources.AddRange(page.Items);
                     return true;
                 });

                return resources;
            }

            public Task Sort(string[] environmentIdsInOrder)
            {
                return Client.Put(Client.RootDocument.Link("EnvironmentSortOrder"), environmentIdsInOrder);
            }

            public Task<EnvironmentEditor> CreateOrModify(string name)
            {
                return new EnvironmentEditor(this).CreateOrModify(name);
            }

            public Task<EnvironmentEditor> CreateOrModify(string name, string description)
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

            public Task<ResourceCollection<EventResource>> List(int skip = 0, string filterByUserId = null, string regardingDocumentId = null, bool includeInternalEvents = false)
            {
                return Client.List<EventResource>(Client.RootDocument.Link("Events"), new { skip, user = filterByUserId, regarding = regardingDocumentId, @internal = includeInternalEvents.ToString() });
            }
        }

        class InterruptionRepository : BasicRepository<InterruptionResource>, IInterruptionRepository
        {
            public InterruptionRepository(IOctopusClient client)
                : base(client, "Interruptions")
            {
            }

            public Task<ResourceCollection<InterruptionResource>> List(int skip = 0, bool pendingOnly = false, string regardingDocumentId = null)
            {
                return Client.List<InterruptionResource>(Client.RootDocument.Link("Interruptions"), new { skip, pendingOnly, regarding = regardingDocumentId });
            }

            public Task Submit(InterruptionResource interruption)
            {
                return Client.Post(interruption.Link("Submit"), interruption.Form.Values);
            }

            public Task TakeResponsibility(InterruptionResource interruption)
            {
                return Client.Put(interruption.Link("Responsible"), (InterruptionResource)null);
            }

            public Task<UserResource> GetResponsibleUser(InterruptionResource interruption)
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

            public async Task<List<ProjectResource>> GetProjects(ProjectGroupResource projectGroup)
            {
                var resources = new List<ProjectResource>();

                await Client.Paginate<ProjectResource>(projectGroup.Link("ProjectGroups"), new { }, page =>
                 {
                     resources.AddRange(page.Items);
                     return true;
                 }).ConfigureAwait(false);

                return resources;
            }

            public Task<ProjectGroupEditor> CreateOrModify(string name)
            {
                return new ProjectGroupEditor(this).CreateOrModify(name);
            }

            public Task<ProjectGroupEditor> CreateOrModify(string name, string description)
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

            public Task<TaskResource> ExecuteHealthCheck(string description = null, int timeoutAfterMinutes = 5, int machineTimeoutAfterMinutes = 1, string environmentId = null, string[] machineIds = null)
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

            public Task<TaskResource> ExecuteCalamariUpdate(string description = null, string[] machineIds = null)
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

            public Task<TaskResource> ExecuteBackup(string description = null)
            {
                var resource = new TaskResource();
                resource.Name = BuiltInTasks.Backup.Name;
                resource.Description = string.IsNullOrWhiteSpace(description) ? "Manual backup" : description;
                return Create(resource);
            }

            public Task<TaskResource> ExecuteTentacleUpgrade(string description = null, string environmentId = null, string[] machineIds = null)
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

            public Task<TaskResource> ExecuteAdHocScript(string scriptBody, string[] machineIds = null, string[] environmentIds = null, string[] targetRoles = null, string description = null, string syntax = "PowerShell")
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

            public Task<TaskDetailsResource> GetDetails(TaskResource resource)
            {
                return Client.Get<TaskDetailsResource>(resource.Link("Details"));
            }

            public Task<string> GetRawOutputLog(TaskResource resource)
            {
                return Client.Get<string>(resource.Link("Raw"));
            }

            public Task Rerun(TaskResource resource)
            {
                return Client.Post(resource.Link("Rerun"), (TaskResource)null);
            }

            public Task Cancel(TaskResource resource)
            {
                return Client.Post(resource.Link("Cancel"), (TaskResource)null);
            }

            public Task WaitForCompletion(TaskResource task, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            {
                return WaitForCompletion(new[] { task }, pollIntervalSeconds, timeoutAfterMinutes, interval);
            }

            public Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Action<TaskResource[]> interval = null)
            {
                Func<TaskResource[], Task> taskInterval = null;
                if (interval != null)
                    taskInterval = tr => Task.Run(() => interval(tr));

                return WaitForCompletion(tasks, pollIntervalSeconds, timeoutAfterMinutes, taskInterval);
            }

            public async Task WaitForCompletion(TaskResource[] tasks, int pollIntervalSeconds = 4, int timeoutAfterMinutes = 0, Func<TaskResource[], Task> interval = null)
            {
                var start = Stopwatch.StartNew();
                if (tasks == null || tasks.Length == 0)
                    return;

                while (true)
                {
                    var stillRunning = await Task.WhenAll(
                            tasks.Select(t => Client.Get<TaskResource>(t.Link("Self")))
                        )
                        .ConfigureAwait(false);

                    if (interval != null)
                        await interval(stillRunning).ConfigureAwait(false);

                    if (stillRunning.All(t => t.IsCompleted))
                        return;

                    if (timeoutAfterMinutes > 0 && start.Elapsed.TotalMinutes > timeoutAfterMinutes)
                    {
                        throw new TimeoutException($"One or more tasks did not complete before the timeout was reached. We waited {start.Elapsed.TotalMinutes:n1} minutes for the tasks to complete.");
                    }

                    Thread.Sleep(pollIntervalSeconds * 1000);
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

            public async Task<UserResource> Register(RegisterCommand registerCommand)
            {
                await Client.Post(Client.RootDocument.Link("Register"), registerCommand).ConfigureAwait(false);
                return await GetCurrent().ConfigureAwait(false);
            }

            public Task SignIn(LoginCommand loginCommand)
            {
                return Client.Post(Client.RootDocument.Link("SignIn"), loginCommand);
            }

            public Task SignOut()
            {
                return Client.Post(Client.RootDocument.Link("SignOut"));
            }

            public Task<UserResource> GetCurrent()
            {
                return Client.Get<UserResource>(Client.RootDocument.Link("CurrentUser"));
            }

            public Task<UserPermissionSetResource> GetPermissions(UserResource user)
            {
                if (user == null) throw new ArgumentNullException("user");
                return Client.Get<UserPermissionSetResource>(user.Link("Permissions"));
            }

            public Task<ApiKeyResource> CreateApiKey(UserResource user, string purpose = null)
            {
                if (user == null) throw new ArgumentNullException("user");
                return Client.Post<object, ApiKeyResource>(user.Link("ApiKeys"), new
                {
                    Purpose = purpose ?? "Requested by Octopus.Client"
                });
            }

            public async Task<List<ApiKeyResource>> GetApiKeys(UserResource user)
            {
                if (user == null) throw new ArgumentNullException("user");
                var resources = new List<ApiKeyResource>();

                await Client.Paginate<ApiKeyResource>(user.Link("ApiKeys"), page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                }).ConfigureAwait(false);

                return resources;
            }

            public Task RevokeApiKey(ApiKeyResource apiKey)
            {
                return Client.Delete(apiKey.Link("Self"));
            }

            public Task<InvitationResource> Invite(string addToTeamId)
            {
                if (addToTeamId == null) throw new ArgumentNullException("addToTeamId");
                return Invite(new ReferenceCollection { addToTeamId });
            }

            public Task<InvitationResource> Invite(ReferenceCollection addToTeamIds)
            {
                return invitations.Create(new InvitationResource { AddToTeamIds = addToTeamIds ?? new ReferenceCollection() });
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

            public Task<ChannelResource> FindByName(ProjectResource project, string name)
            {
                return FindByName(name, path: project.Link("Channels"));
            }

            public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name)
            {
                return new ChannelEditor(this).CreateOrModify(project, name);
            }

            public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description)
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

            public Task<ProjectTriggerResource> FindByName(ProjectResource project, string name)
            {
                return FindByName(name, path: project.Link("Triggers"));
            }

            public Task<ProjectTriggerEditor> CreateOrModify(ProjectResource project, string name, ProjectTriggerType type)
            {
                return new ProjectTriggerEditor(this).CreateOrModify(project, name, type);
            }
        }

        class SchedulerRepository : ISchedulerRepository
        {
            readonly IOctopusClient client;

            public SchedulerRepository(IOctopusClient client)
            {
                this.client = client;
            }

            public Task Start()
            {
                return client.GetContent("~/api/scheduler/start");
            }

            public Task Start(string taskName)
            {
                return client.GetContent($"~/api/scheduler/start?task={taskName}");
            }

            public Task Trigger(string taskName)
            {
                return client.GetContent($"~/api/scheduler/trigger?task={taskName}");
            }

            public Task Stop()
            {
                return client.GetContent("~/api/scheduler/stop");
            }

            public Task Stop(string taskName)
            {
                return client.GetContent($"~/api/scheduler/stop?task={taskName}");
            }
        }

        class TenantRepository : BasicRepository<TenantResource>, ITenantRepository
        {
            public TenantRepository(IOctopusClient client)
                : base(client, "Tenants")
            {
            }

            public Task<TenantVariableResource> GetVariables(TenantResource tenant)
            {
                return Client.Get<TenantVariableResource>(tenant.Link("Variables"));
            }

            public Task<List<TenantResource>> FindAll(string name, string[] tags)
            {
                return Client.Get<List<TenantResource>>(Client.RootDocument.Link("Tenants"), new { id = "all", name, tags });
            }

            public Task<TenantVariableResource> ModifyVariables(TenantResource tenant, TenantVariableResource variables)
            {
                return Client.Post<TenantVariableResource, TenantVariableResource>(tenant.Link("Variables"), variables);
            }

            public Task<List<TenantsMissingVariablesResource>> GetMissingVariables(string tenantId = null, string projectId = null, string environmentId = null)
            {
                return Client.Get<List<TenantsMissingVariablesResource>>(Client.RootDocument.Link("TenantsMissingVariables"), new
                {
                    tenantId = tenantId,
                    projectId = projectId,
                    environmentId = environmentId
                });
            }

            public Task SetLogo(TenantResource tenant, string fileName, Stream contents)
            {
                return Client.Post(tenant.Link("Logo"), new FileUpload { Contents = contents, FileName = fileName }, false);
            }

            public Task<TenantEditor> CreateOrModify(string name)
            {
                return new TenantEditor(this).CreateOrModify(name);
            }
        }

        class TagSetRepository : BasicRepository<TagSetResource>, ITagSetRepository
        {
            public TagSetRepository(IOctopusClient client) : base(client, "TagSets")
            {
            }

            public Task Sort(string[] tagSetIdsInOrder)
            {
                return Client.Put(Client.RootDocument.Link("TagSetSortOrder"), tagSetIdsInOrder);
            }

            public Task<TagSetEditor> CreateOrModify(string name)
            {
                return new TagSetEditor(this).CreateOrModify(name);
            }

            public Task<TagSetEditor> CreateOrModify(string name, string description)
            {
                return new TagSetEditor(this).CreateOrModify(name, description);
            }
        }
    }
}