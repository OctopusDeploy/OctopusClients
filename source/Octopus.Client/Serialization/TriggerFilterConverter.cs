using System;
using System.Collections.Generic;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Serialization
{
    public class TriggerFilterConverter : InheritedClassConverter<TriggerFilterResource, TriggerFilterType>
    {
        static readonly IDictionary<TriggerFilterType, Type> FilterTypes =
          new Dictionary<TriggerFilterType, Type>
          {
              { TriggerFilterType.MachineFilter, typeof (MachineFilterResource)}
          };

        protected override IDictionary<TriggerFilterType, Type> DerivedTypeMappings => FilterTypes;
        protected override string TypeDesignatingPropertyName => "FilterType";
    }
}