using NuGet.Versioning;

namespace Octopus.Client.Model
{
    public partial class SemanticVersion
    {
        public NuGetVersion ToNuGetVersion()
        {
            return new NuGetVersion(this.Version, this.ReleaseLabels, this.Metadata, this._originalString);
        }

        public static SemanticVersion FromNuGetVersion(NuGetVersion nugetVersion)
        {
            return new SemanticVersion(nugetVersion.Version, nugetVersion.ReleaseLabels, nugetVersion.Metadata, nugetVersion.ToString());    
        }
    }
}