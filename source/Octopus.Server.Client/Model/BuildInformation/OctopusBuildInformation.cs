using Octopus.Client.Model.IssueTrackers;

namespace Octopus.Client.Model.BuildInformation
{
    public class OctopusBuildInformation
    {
        internal static string BuildInformationRequiresOctopusVersion = "2019.9.0";
        internal static string BuildInformationRequiresOctopusVersionMessage = $"Pushing build information/package metadata requires Octopus version {BuildInformationRequiresOctopusVersion} or newer";

        public OctopusBuildInformation()
        {
            Commits = new Commit[0];
        }
        
        public string BuildEnvironment { get; set; }
        public string BuildNumber { get; set; }
        public string BuildUrl { get; set; }
        public string Branch { get; set; }
        public string VcsType { get; set; }
        public string VcsRoot { get; set; }
        public string VcsCommitNumber { get; set; }

        public Commit[] Commits { get; set; }
    }
}