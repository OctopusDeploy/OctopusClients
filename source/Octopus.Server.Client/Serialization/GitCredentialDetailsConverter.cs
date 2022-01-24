using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Model.GitCredentials;

namespace Octopus.Client.Serialization
{
    public class GitCredentialDetailsConverter : InheritedClassConverter<GitCredentialDetails, GitCredentialDetailsType>
    {
        static readonly IDictionary<GitCredentialDetailsType, Type> GitCredentialDetailsTypeMappings =
            new Dictionary<GitCredentialDetailsType, Type>
            {
                {GitCredentialDetailsType.UsernamePassword, typeof(UsernamePasswordGitCredentialDetails)}
            };

        static readonly Type defaultType = typeof(AnonymousVersionControlCredentialsResource);

        protected override IDictionary<GitCredentialDetailsType, Type> DerivedTypeMappings => GitCredentialDetailsTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(GitCredentialDetails.Type);

        protected override Type DefaultType { get; } = defaultType;
    }
}