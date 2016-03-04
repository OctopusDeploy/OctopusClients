using System;
using System.Collections.Generic;
using NuGet;

namespace Octopus.Cli.Commands
{
    public interface IPackageBuilder
    {
        void BuildPackage(string basePath, IList<string> includes, ManifestMetadata metadata, string outFolder, bool overwrite);
    }
}