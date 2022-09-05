using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class SnapshotGitReferenceResource
    {
        [WriteableOnCreate]
        public string GitRef { get; set; }
        public string GitCommit { get; set; }
        public string VariablesGitCommit { get; set; }
    }
}