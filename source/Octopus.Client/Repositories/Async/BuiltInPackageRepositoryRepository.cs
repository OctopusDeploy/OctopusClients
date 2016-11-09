using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IBuiltInPackageRepositoryRepository
    {
        Task<PackageFromBuiltInFeedResource> PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        Task<ResourceCollection<PackageFromBuiltInFeedResource>> ListPackages(string packageId, int skip = 0, int take = 30);
        Task<ResourceCollection<PackageFromBuiltInFeedResource>> LatestPackages(int skip = 0, int take = 30);
        Task DeletePackage(PackageResource package);
    }

    class BuiltInPackageRepositoryRepository : IBuiltInPackageRepositoryRepository
    {
        readonly IOctopusAsyncClient client;

        public BuiltInPackageRepositoryRepository(IOctopusAsyncClient client)
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
}
