using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.VersionControl
{
    public class VersionControlReferenceResource
    {
        [WriteableOnCreate]
        public string GitRef { get; set; }
        public string GitCommit { get; set; }
    }
}