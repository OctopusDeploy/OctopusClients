using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public class RunRunbookActionResource : ScopedDeploymentActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.RunRunbook;

        [Writeable] 
        public string RunbookId { get; set; }
        
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }
        
    }
}