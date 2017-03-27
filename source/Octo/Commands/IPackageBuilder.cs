using System.Collections.Generic;
using ManifestMetadata = NuGet.Packaging.ManifestMetadata;

namespace Octopus.Cli.Commands
{
    public interface IPackageBuilder
    {
        void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite, bool verboseInfo);
    }
}