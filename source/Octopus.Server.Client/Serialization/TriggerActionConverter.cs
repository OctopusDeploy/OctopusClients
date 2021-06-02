using System;
using System.Collections.Generic;
using Octopus.Client.Model.Triggers;
using Octopus.Client.Model.Triggers.ScheduledTriggers;

namespace Octopus.Client.Serialization
{
    public class TriggerActionConverter : InheritedClassConverter<TriggerActionResource, TriggerActionType>
    {
        static readonly IDictionary<TriggerActionType, Type> ActionTypes =
            new Dictionary<TriggerActionType, Type>
            {
                {TriggerActionType.AutoDeploy, typeof(AutoDeployActionResource)},
                {TriggerActionType.DeployLatestRelease, typeof(DeployLatestReleaseActionResource)},
                {TriggerActionType.DeployNewRelease, typeof(DeployNewReleaseActionResource)},
                {TriggerActionType.RunRunbook, typeof(RunRunbookActionResource)}
            };

        protected override IDictionary<TriggerActionType, Type> DerivedTypeMappings => ActionTypes;
        protected override string TypeDesignatingPropertyName => "ActionType";
    }
}