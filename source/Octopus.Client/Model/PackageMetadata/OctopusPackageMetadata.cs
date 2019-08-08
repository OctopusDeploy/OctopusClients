using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model.PackageMetadata
{
    /// <summary>
    /// This class is what the build server extensions will create the Json content for. 
    /// </summary>
    public class OctopusPackageMetadata
    {
        public static string PackageMetadataRequiresOctopusVersion = "Pushing build information/package metadata requires Octopus version 2019.4.1 or newer";

        public OctopusPackageMetadata()
        {
            Commits = new Commit[0];
        }
        
        public string BuildEnvironment { get; set; }
        public string CommentParser { get; set; }
        public string BuildNumber { get; set; }
        public string BuildUrl { get; set; }
        public string VcsType { get; set; }
        public string VcsRoot { get; set; }
        public string VcsCommitNumber { get; set; }

        public Commit[] Commits { get; set; }
    }
}