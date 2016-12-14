using System;
using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBuiltInPackageRepositoryRepository
    {
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30);
        ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30);
        void DeletePackage(PackageResource package);
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
                new FileUpload() { Contents = contents, FileName = fileName },
                new { replace = replaceExisting });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.RootDocument.Link("Packages"), new { latest = true, take, skip });
        }

        public void DeletePackage(PackageResource package)
        {
            client.Delete(client.RootDocument.Link("Packages"), new { id = package.Id });
        }
    }
}