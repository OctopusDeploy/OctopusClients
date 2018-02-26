using System;
using System.Collections.Generic;
using Octopus.Client.Model.ScheduledTriggers;

namespace Octopus.Client.Serialization
{
    public class ScheduledTriggerActionConverter : InheritedClassConverter<ScheduledTriggerActionResource, ScheduledTriggerActionType>
    {
        static readonly IDictionary<ScheduledTriggerActionType, Type> ActionTypes =
            new Dictionary<ScheduledTriggerActionType, Type>
            {
                { ScheduledTriggerActionType.DeployNewRelease, typeof (DeployNewReleaseActionResource)},
                { ScheduledTriggerActionType.DeployLatestRelease, typeof (DeployLatestReleaseActionResource)},
            };

        protected override IDictionary<ScheduledTriggerActionType, Type> DerivedTypeMappings => ActionTypes;
        protected override string TypeDesignatingPropertyName => "ActionType";
    }
}
