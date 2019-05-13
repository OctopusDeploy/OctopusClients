using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ILicensesRepository
    {
        Task<LicenseResource> GetCurrent();
        Task<LicenseResource> UpdateCurrent(LicenseResource resource);
        Task<LicenseStatusResource> GetStatus();
    }
    
    class LicensesRepository : BasicRepository<LicenseResource>, ILicensesRepository
    {
        public LicensesRepository(IOctopusAsyncRepository repository)
            : base(repository, "CurrentLicense")
        {
        }

        public async Task<LicenseResource> GetCurrent()
            => await Client.Get<LicenseResource>(await Repository.Link(CollectionLinkName));

        public async Task<LicenseResource> UpdateCurrent(LicenseResource resource)
            => await Client.Update(await Repository.Link(CollectionLinkName), resource);

        public async Task<LicenseStatusResource> GetStatus()
            => await Client.Get<LicenseStatusResource>(await Repository.Link("CurrentLicenseStatus"));

    }
}