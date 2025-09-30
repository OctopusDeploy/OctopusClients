using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents an environment. Environments are user-defined and map to real world deployment environments
    /// such as development, staging, test and production. Projects are deployed to environments.
    /// </summary>
    public class EnvironmentResource : ResourceWithExtensionSettings, INamedResource, IHaveSpaceResource, IHaveSlugResource
    {
        /// <summary>
        /// Gets or sets the name of this environment. This should be short, preferably 5-20 characters.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a short description of this environment that can be used to explain the purpose of
        /// the environment to other users. This field may contain markdown.
        /// </summary>
        [Writeable]
        [Trim]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a number indicating the priority of this environment in sort order. Environments with
        /// a lower sort order will appear in the UI before items with a higher sort order.
        /// </summary>
        [Writeable]
        public int SortOrder { get; set; }

        /// <summary>
        /// If set to true, deployments will prompt for manual intervention (Fail/Retry/Ignore) when
        /// failures are encountered in activities that support it.
        /// </summary>
        [Writeable]
        public bool UseGuidedFailure { get; set; }

        /// <summary>
        /// If set to true, deployments to this environment will be allowed to contain steps that manage infrastructure.
        /// </summary>
        [Writeable]
        public bool AllowDynamicInfrastructure { get; set; }

        public string SpaceId { get; set; }

        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets the list of tags assigned to this environment.
        /// </summary>
        [Writeable]
        public List<string> EnvironmentTags { get; set; } = new List<string>();

        public class IdComparer : IEqualityComparer<EnvironmentResource>
        {
            public bool Equals(EnvironmentResource x, EnvironmentResource y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Id == y.Id;
            }

            public int GetHashCode(EnvironmentResource obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }
    }
}