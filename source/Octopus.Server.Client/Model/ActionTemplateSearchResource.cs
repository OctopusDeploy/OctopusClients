#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionTemplateSearchResource : IResource
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ActionTemplateSearchResource()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        {
            Categories = new List<string>();
        }

        private string legacyCategory;

        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Author { get; set; }
        public string? Website { get; set; }
        public bool IsInstalled { get; set; }
        public bool IsBuiltIn { get; set; }
        public string? SpaceId { get; set; }

        // @BackwardsCompat: This property was deprecated to support multiple category selection. Please use the Categories property instead.
        public string Category
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => string.IsNullOrEmpty(legacyCategory) ? Categories?.FirstOrDefault() : legacyCategory;
#pragma warning restore CS8603 // Possible null reference return.
            set => legacyCategory = value;
        }
        public List<string> Categories { get; set; }

        public IEnumerable<string> Features { get; set; }

        public string CommunityActionTemplateId { get; set; }
        public bool HasUpdate { get; set; }

        public LinkCollection Links { get; set; }
    }
}
