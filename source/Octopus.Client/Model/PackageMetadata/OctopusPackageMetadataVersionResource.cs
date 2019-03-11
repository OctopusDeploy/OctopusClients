
namespace Octopus.Client.Model.PackageMetadata
{
    public class OctopusPackageMetadataVersionResource : Resource
    {
        public string PackageId { get; set; }
        public string Version { get; set; }

        public OctopusPackageMetadata OctopusPackageMetadata { get; set; }
    }
}