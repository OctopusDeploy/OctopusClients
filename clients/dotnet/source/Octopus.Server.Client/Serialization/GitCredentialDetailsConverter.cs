using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.GitCredentials;

namespace Octopus.Client.Serialization
{
    public class GitCredentialDetailsConverter : InheritedClassConverter<GitCredentialDetails, GitCredentialType>
    {
        static readonly IDictionary<GitCredentialType, Type> GitCredentialDetailsTypeMappings =
            new Dictionary<GitCredentialType, Type>
            {
                {GitCredentialType.UsernamePassword, typeof(UsernamePasswordGitCredentialDetails)}
            };

        static readonly Type defaultType = typeof(AnonymousProjectGitCredentialResource);

        protected override IDictionary<GitCredentialType, Type> DerivedTypeMappings => GitCredentialDetailsTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(GitCredentialDetails.Type);

        protected override Type DefaultType { get; } = defaultType;
    }
}