using System;

namespace Octopus.Client.Model
{
    public class MachineDeploymentPreview
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool HasLatestCalamari { get; set; }
    }
}