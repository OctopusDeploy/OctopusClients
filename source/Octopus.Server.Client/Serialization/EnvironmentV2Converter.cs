using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    class EnvironmentV2Converter : InheritedClassConverter<BaseEnvironmentV2Resource, EnvironmentType>
    {
        static readonly IDictionary<EnvironmentType, Type> EnvironmentV2Types =
            new Dictionary<EnvironmentType, Type>
            {
                { EnvironmentType.Static, typeof(StaticEnvironmentV2Resource) },
                { EnvironmentType.Parent, typeof(ParentEnvironmentV2Resource) },
                { EnvironmentType.Ephemeral, typeof(EphemeralEnvironmentV2Resource) }
            };

        protected override IDictionary<EnvironmentType, Type> DerivedTypeMappings => EnvironmentV2Types;
        protected override string TypeDesignatingPropertyName => "Type";
    }
}
