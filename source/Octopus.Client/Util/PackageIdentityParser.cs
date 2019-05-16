using System.Text.RegularExpressions;
using Octopus.Client.Model;

namespace Octopus.Client.Util
{
    
    // From OctopusServer's PackageIdentity class
    public class PackageIdentityParser
    {
        /// <summary>
        /// Takes a string containing a concatenated package ID and version (e.g. a filename or database-key) and 
        /// attempts to parse a package ID and semantic version.  
        /// </summary>
        /// <param name="idAndVersion">The concatenated package ID and version.</param>
        /// <param name="packageId">The parsed package ID</param>
        /// <param name="version">The parsed semantic version</param>
        /// <returns>True if parsing was successful, else False</returns>
        public static bool TryParsePackageIdAndVersion(
            string idAndVersion,
            out string packageId,
            out SemanticVersion version)
        {
            packageId = null;
            version = null;

            const string packageIdPattern = @"(?<packageId>(\w+([_.-]\w+)*?))";
            const string semanticVersionPattern = @"(?<semanticVersion>(\d+(\.\d+){0,3}" // Major Minor Patch
                + @"(-[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)" // Pre-release identifiers
                + @"(\+[0-9A-Za-z-]+(\.[0-9A-Za-z-]+)*)?)"; // Build Metadata

            var packageIdAndVersionPattern = $@"^{packageIdPattern}\.{semanticVersionPattern}$";

            var match = Regex.Match(idAndVersion, packageIdAndVersionPattern);
            var packageIdMatch = match.Groups["packageId"];
            var versionMatch = match.Groups["semanticVersion"];

            if (!packageIdMatch.Success)
                return false;

            packageId = packageIdMatch.Value;
            return versionMatch.Success && SemanticVersion.TryParse(versionMatch.Value, out version);
        }
    }
}