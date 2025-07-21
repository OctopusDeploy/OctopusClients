using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionTemplateSearchResource : IResource
    {
        public ActionTemplateSearchResource()
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
        public string Website { get; set; }
        public bool IsInstalled { get; set; }
        public bool IsBuiltIn { get; set; }

        // @BackwardsCompat: This property was deprecated to support multiple category selection. Please use the Categories property instead.
        public string Category
        {
            get => string.IsNullOrEmpty(legacyCategory) ? Categories?.FirstOrDefault() : legacyCategory;
            set => legacyCategory = value;
        }
        public List<string> Categories { get; set; }

        public IEnumerable<string> Features { get; set; }

        public string CommunityActionTemplateId { get; set; }
        public bool HasUpdate { get; set; }

        public LinkCollection Links { get; set; }
    }
}