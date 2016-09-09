using System;
using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IBuiltInPackageRepositoryRepository
    {
        PackageResource PushPackage(string fileName, Stream contents, bool replaceExisting = false);
        ResourceCollection<PackageResource> ListPackages(string packageId, int skip = 0, int take = 30);
        ResourceCollection<PackageResource> LatestPackages(int skip = 0, int take = 30);
        void DeletePackage(PackageResource package);
    }
}