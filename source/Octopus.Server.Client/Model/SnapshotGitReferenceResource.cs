using Octopus.Client.Model.Git;

namespace Octopus.Client.Model
{
    public class SnapshotGitReferenceResource : GitReferenceResource
    {
        public string VariablesGitCommit { get; set; }
    }
}