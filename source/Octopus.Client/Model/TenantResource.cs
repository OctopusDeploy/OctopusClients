using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TenantResource : Resource, INamedResource, IHaveSpaceResource
    {
        public TenantResource()
        {
            ProjectEnvironments = new Dictionary<string, ReferenceCollection>();
            TenantTags = new ReferenceCollection();
        }

        [Writeable]
        public string Name { get; set; }

        /// <summary>
        /// Tags are referenced by CanonicalName like {TagSetName}/{TagName}
        /// </summary>
        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        [Writeable]
        public IDictionary<string, ReferenceCollection> ProjectEnvironments { get; set; }

        public TenantResource WithTag(TagResource tag)
        {
            if (!TenantTags.Any(t => string.Equals(t, tag.CanonicalTagName, StringComparison.OrdinalIgnoreCase)))
            {
                TenantTags.Add(tag.CanonicalTagName);
            }

            return this;
        }

        public TenantResource ClearTags()
        {
            TenantTags.Clear();
            return this;
        }

        public TenantResource ConnectToProjectAndEnvironments(ProjectResource project, params EnvironmentResource[] environments)
        {
            ReferenceCollection existing;
            if (ProjectEnvironments.TryGetValue(project.Id, out existing))
            {
                existing.ReplaceAll(environments.Select(e => e.Id));
            }
            else
            {
                ProjectEnvironments.Add(project.Id, new ReferenceCollection(environments.Select(e => e.Id)));
            }

            return this;
        }

        public TenantResource ClearProjects()
        {
            ProjectEnvironments.Clear();
            return this;
        }

        public string SpaceId { get; set; }
    }
}
