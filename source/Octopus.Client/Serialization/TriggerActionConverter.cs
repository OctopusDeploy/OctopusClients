using System;
using System.Collections.Generic;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Serialization
{
    public class TriggerActionConverter : InheritedClassConverter<TriggerActionResource, TriggerActionType>
    {
        static readonly IDictionary<TriggerActionType, Type> ActionTypes =
          new Dictionary<TriggerActionType, Type>
          {
              { TriggerActionType.AutoDeploy, typeof (AutoDeployActionResource)}
          };

        protected override IDictionary<TriggerActionType, Type> DerivedTypeMappings => ActionTypes;
        protected override string TypeDesignatingPropertyName => "ActionType";
    }
}