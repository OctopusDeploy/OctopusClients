using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class OctopusServerClusterSummaryResource
    {
        public OctopusServerNodeSummaryResource[] Nodes { get; set; }
        public LinkCollection Links { get; set; }
    }
}
