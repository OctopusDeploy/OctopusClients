using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class VersionControlSettingsConverter : InheritedClassConverter<VersionControlCredentialsResource, VersionControlCredentialsType>
    {
        static readonly IDictionary<VersionControlCredentialsType, Type> VersionControlCredentialTypeMappings =
            new Dictionary<VersionControlCredentialsType, Type>
            {
                {VersionControlCredentialsType.Anonymous, typeof(AnonymousVersionControlCredentialsResource)},
                {VersionControlCredentialsType.UsernamePassword, typeof(UsernamePasswordVersionControlCredentialsResource)},
                {VersionControlCredentialsType.Reference, typeof(ReferenceVersionControlCredentialsResource)},
            };

        static readonly Type defaultType = typeof(AnonymousVersionControlCredentialsResource);

        protected override IDictionary<VersionControlCredentialsType, Type> DerivedTypeMappings => VersionControlCredentialTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(VersionControlCredentialsResource.Type);

        protected override Type DefaultType { get; } = defaultType;
    }
}