using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model.PackageMetadata
{
    /// <summary>
    /// This class is what the build server extensions will create the Json content for. 
    /// </summary>
    public class OctopusPackageMetadata
    {
        public string BuildEnvironment { get; set; }
        public string CommentParser { get; set; }
        public string BuildNumber { get; set; }
        public string BuildLink { get; set; }
        public string VcsRoot { get; set; }
        public string VcsCommitNumber { get; set; }

        public Commit[] Commits { get; set; }
    }
}