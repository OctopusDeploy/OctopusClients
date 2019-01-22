using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Project groups are used to organize collections of related projects. For example, a customer may have a project
    /// group named
    /// "Corporate Website" containing three seperate projects. Project groups affect retention policies and permissions.
    /// </summary>
    public class ProjectGroupResource : Resource, INamedResource, IHaveSpaceResource
    {
        public ProjectGroupResource()
        {
            EnvironmentIds = new ReferenceCollection();
        }

        /// <summary>
        /// Gets or sets the name of this project group.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a collection of environment ID's. If this collection is empty, projects in this group can be deployed
        /// to any environment. If the collection is non-empty, then projects in the group are limited to only deploying to the
        /// environments listed in this collection.
        /// </summary>
        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        /// <summary>
        /// Gets or sets the ID of the retention policy that will apply to projects in this group.
        /// </summary>
        [Writeable]
        public string RetentionPolicyId { get; set; }

        public string SpaceId { get; set; }
    }
}