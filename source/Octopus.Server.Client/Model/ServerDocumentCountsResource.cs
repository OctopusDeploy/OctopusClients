namespace Octopus.Client.Model
{
    public class ServerDocumentCountsResource : Resource
    {
        public int Spaces { get; set; }
        public int Users { get; set; }
        public int Teams { get; set; }
        public int Environments { get; set; }
        public int Tenants { get; set; }
        public int DeploymentTargets { get; set; }
        public int Workers { get; set; }
        public int WorkerPools { get; set; }
        public int Projects { get; set; }
        public int Releases { get; set; }
        public int Deployments { get; set; }
        public int Runbooks { get; set; }
        public int RunbookRuns { get; set; }
        public int Certificates { get; set; }
        public int VariableSets { get; set; }
        public int Packages { get; set; }
    }
}