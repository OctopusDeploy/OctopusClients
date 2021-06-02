using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeDetailsResource : Resource
    {
        [Writeable]
        public int RunningTasks { get; set; }
    }
}