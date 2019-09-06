using Octopus.Client.Extensibility;

namespace Octopus.Client.Model.Processes
{
    public abstract class ProcessResource : Resource, INamedResource, IHaveSpaceResource
    {
        public ProcessResource()
        {
        }

        [Trim]
        public string Name { get; set; }

        public abstract ProcessType Type { get; }

        public string StepsId { get; set; }

        public string ProjectId { get; set; }

        public string SpaceId { get; set; }
    }
}