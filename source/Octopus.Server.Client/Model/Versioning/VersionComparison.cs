// Based on VersionComparison from https://github.com/NuGet/NuGet.Client
// NuGet is licensed under the Apache license: https://github.com/NuGet/NuGet.Client/blob/dev/LICENSE.txt

namespace Octopus.Client.Model.Versioning
{
    /// <summary>
    /// Version comparison modes.
    /// </summary>
    public enum VersionComparison
    {
        /// <summary>
        /// By default all components (inc. build metadata) will be used in the compare.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Compares only the version numbers.
        /// </summary>
        Version = 1,

        /// <summary>
        /// Include Version number and Release labels in the compare.
        /// </summary>
        VersionRelease = 2,

        /// <summary>
        /// Include all metadata during the compare.
        /// </summary>
        VersionReleaseMetadata = 3
    }
}