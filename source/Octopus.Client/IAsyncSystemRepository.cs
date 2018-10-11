using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    public interface IAsyncSystemRepository
    {
        IBackupRepository Backups { get; }
        ICommunityActionTemplateRepository CommunityActionTemplates { get; }
        IConfigurationRepository Configuration { get; }
        IFeaturesConfigurationRepository FeaturesConfiguration { get; }
        IMigrationRepository Migrations { get; }
        IOctopusServerNodeRepository OctopusServerNodes { get; }
        IPerformanceConfigurationRepository PerformanceConfiguration { get; }
        ISchedulerRepository Schedulers { get; }
        IServerStatusRepository ServerStatus { get; }
        IUserRepository Users { get; }
        IUserRolesRepository UserRoles { get; }
    }
}