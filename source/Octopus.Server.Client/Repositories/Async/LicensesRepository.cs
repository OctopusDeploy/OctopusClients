using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ILicensesRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LicenseResource> GetCurrent();
        Task<LicenseResource> GetCurrent(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LicenseResource> UpdateCurrent(LicenseResource resource);
        Task<LicenseResource> UpdateCurrent(LicenseResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LicenseStatusResource> GetStatus();
        Task<LicenseStatusResource> GetStatus(CancellationToken cancellationToken);
    }

    class LicensesRepository : BasicRepository<LicenseResource>, ILicensesRepository
    {
        public LicensesRepository(IOctopusAsyncRepository repository)
            : base(repository, "CurrentLicense")
        {
        }

        public Task<LicenseResource> GetCurrent()
            => GetCurrent(CancellationToken.None);

        public async Task<LicenseResource> GetCurrent(CancellationToken cancellationToken)
            => await Client.Get<LicenseResource>(await Repository.Link(CollectionLinkName), cancellationToken);

        public Task<LicenseResource> UpdateCurrent(LicenseResource resource)
            => UpdateCurrent(resource, CancellationToken.None);

        public async Task<LicenseResource> UpdateCurrent(LicenseResource resource, CancellationToken cancellationToken)
            => await Client.Update(await Repository.Link(CollectionLinkName), resource, cancellationToken);

        public Task<LicenseStatusResource> GetStatus()
            => GetStatus(CancellationToken.None);

        public async Task<LicenseStatusResource> GetStatus(CancellationToken cancellationToken)
            => await Client.Get<LicenseStatusResource>(await Repository.Link("CurrentLicenseStatus"), cancellationToken);

    }
}
