using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBuiltInPackageRepositoryRepository
    {
        PackageFromBuiltInFeedResource PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30);
        ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30);
        void DeletePackage(PackageResource package);
        void DeletePackages(IReadOnlyList<PackageResource> packages);
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
                client.Link("PackageUpload"),
                new FileUpload() { Contents = contents, FileName = fileName },
                new { replace = replaceExisting });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> ListPackages(string packageId, int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.Link("Packages"), new { nuGetPackageId = packageId, take, skip });
        }

        public ResourceCollection<PackageFromBuiltInFeedResource> LatestPackages(int skip = 0, int take = 30)
        {
            return client.List<PackageFromBuiltInFeedResource>(client.Link("Packages"), new { latest = true, take, skip });
        }

        public void DeletePackage(PackageResource package)
        {
            client.Delete(client.Link("Packages"), new { id = package.Id });
        }

        public void DeletePackages(IReadOnlyList<PackageResource> packages)
            => client.Delete(client.Link("PackagesBulk"), new { ids = packages.Select(p => p.Id).ToArray() });
    }
}