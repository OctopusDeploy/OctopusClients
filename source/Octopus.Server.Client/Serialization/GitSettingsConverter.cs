using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Serialization
{
    public class GitSettingsConverter : InheritedClassConverter<ProjectGitCredentialResource, ProjectGitCredentialType>
    {
        static readonly IDictionary<ProjectGitCredentialType, Type> GitCredentialTypeMappings =
            new Dictionary<ProjectGitCredentialType, Type>
            {
                {ProjectGitCredentialType.Anonymous, typeof(AnonymousProjectGitCredentialResource)},
                {ProjectGitCredentialType.UsernamePassword, typeof(UsernamePasswordProjectGitCredentialResource)},
                {ProjectGitCredentialType.Reference, typeof(ReferenceProjectGitCredentialResource)},
            };

        static readonly Type defaultType = typeof(AnonymousProjectGitCredentialResource);

        protected override IDictionary<ProjectGitCredentialType, Type> DerivedTypeMappings => GitCredentialTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(ProjectGitCredentialResource.Type);

        protected override Type DefaultType { get; } = defaultType;
    }
}