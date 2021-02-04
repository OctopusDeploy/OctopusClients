using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class PersistenceSettingsConverter : InheritedClassConverter<PersistenceSettingsResource, PersistenceSettingsType>
    {
        static readonly IDictionary<PersistenceSettingsType, Type> PersistenceSettingsTypeMappings =
            new Dictionary<PersistenceSettingsType, Type>
            {
                {PersistenceSettingsType.Database, typeof(DatabasePersistenceSettingsResource)},
                {PersistenceSettingsType.VersionControlled, typeof(VersionControlSettingsResource)},
            };

        static readonly Type defaultType = typeof(DatabasePersistenceSettingsResource);

        protected override IDictionary<PersistenceSettingsType, Type> DerivedTypeMappings => PersistenceSettingsTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(PersistenceSettingsResource.Type);

        protected override Type DefaultType { get; } = defaultType;
    }
}