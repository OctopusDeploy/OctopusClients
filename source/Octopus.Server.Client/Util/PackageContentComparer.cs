using System.IO;
using Octopus.Client.Logging;
using Octopus.Client.Model;

namespace Octopus.Client.Util
{
    static class PackageContentComparer
    {
        public static bool AreSame(PackageFromBuiltInFeedResource uploadedPackage, Stream contents, ILog logger)
        {
            if (uploadedPackage == null)
            {
                logger.Info("Package hasn't been uploaded");
                return false;
            }

            var localPackageHash = CalculateLocalPackageHash(contents);
            if (localPackageHash == null)
            {
                logger.Info("Couldn't calculate the hash of the local package");
                return false;
            }

            if (localPackageHash != uploadedPackage.Hash)
            {
                logger.Info("The hash of the local package and the hash of the uploaded package don't match");
                return false;
            }

            logger.Info("Package has been successfully uploaded");

            return true;
        }

        private static string CalculateLocalPackageHash(Stream contents)
        {
            if (contents.CanSeek && contents.CanRead)
            {
                contents.Seek(0, SeekOrigin.Begin);
                return HashCalculator.Hash(contents);
            }

            if (!(contents is FileStream fileStream)) return null;

            // When a full package upload times out we get a stream here that is already disposed so we need to open a new one
            using (var newContents = File.OpenRead(fileStream.Name))
            {
                return HashCalculator.Hash(newContents);
            }
        }

    }
}