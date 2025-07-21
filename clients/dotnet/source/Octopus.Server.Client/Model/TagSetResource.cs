using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TagSetResource : Resource, INamedResource, IHaveSpaceResource
    {
        public TagSetResource()
        {
            Tags = new List<TagResource>();
        }

        /// <summary>
        /// Gets or sets the name of this tag set.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this tag set.
        /// </summary>
        [Writeable]
        [Trim]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the sort order of this tag set.
        /// </summary>
        [Writeable]
        public int SortOrder { get; set; }

        /// <summary>
        /// The tags that make up this tag set
        /// </summary>
        [Writeable]
        public IList<TagResource> Tags { get; set; }

        public TagSetResource AddOrUpdateTag(
            string name,
            string description = null,
            string color = TagResource.StandardColor.DarkGrey)
        {
            var existing = Tags.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                Tags.Add(new TagResource
                {
                    Name = name,
                    Description = description,
                    Color = color
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                existing.Color = color;
            }

            return this;
        }

        public TagSetResource RemoveTag(string name)
        {
            var existing = Tags.FirstOrDefault(t => string.Equals(t.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing != null) Tags.Remove(existing);
            return this;
        }

        public string SpaceId { get; set; }
    }
}