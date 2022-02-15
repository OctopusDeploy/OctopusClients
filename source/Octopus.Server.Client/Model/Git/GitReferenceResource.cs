using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Git
{
    public class GitReferenceResource
    {
        [WriteableOnCreate]
        public string GitRef { get; set; }
        public string GitCommit { get; set; }
    }
}