using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeRunningTasksResource : Resource
    {
        [Writeable]
        public int RunningTasks { get; set; }
    }
}