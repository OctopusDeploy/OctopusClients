using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ILicensesRepository
    {
        LicenseResource GetCurrent();
        LicenseResource UpdateCurrent(LicenseResource resource);
        LicenseStatusResource GetStatus();
    }
    
    class LicensesRepository : BasicRepository<LicenseResource>, ILicensesRepository
    {
        public LicensesRepository(IOctopusRepository repository)
            : base(repository, "CurrentLicense")
        {
        }

        public LicenseResource GetCurrent()
            => Client.Get<LicenseResource>(Repository.Link(CollectionLinkName));

        public LicenseResource UpdateCurrent(LicenseResource resource)
            => Client.Update(Repository.Link(CollectionLinkName), resource);

        public LicenseStatusResource GetStatus()
            => Client.Get<LicenseStatusResource>(Repository.Link("CurrentLicenseStatus"));

    }
}