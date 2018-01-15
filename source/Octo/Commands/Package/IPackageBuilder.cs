using System.Collections.Generic;
using ManifestMetadata = NuGet.Packaging.ManifestMetadata;

namespace Octopus.Cli.Commands.Package
{
    public interface IPackageBuilder
    {
        string[] Files { get; }

        string PackageFormat { get; }

        void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo);
    }
}