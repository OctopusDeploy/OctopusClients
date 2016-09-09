using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class ResourceCollection<TResource> : Resource
    {
        public ResourceCollection(IEnumerable<TResource> items, LinkCollection links)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (links == null) throw new ArgumentNullException("links");
            Items = items.ToList();
            Links = links;
        }

        [JsonProperty(Order = 1)]
        public string ItemType
        {
            get { return typeof (TResource).Name.Replace("Resource", ""); }
        }

        [JsonProperty(Order = 3)]
        public bool IsStale { get; set; }

        [JsonProperty(Order = 4)]
        public int TotalResults { get; set; }

        [JsonProperty(Order = 5)]
        public int ItemsPerPage { get; set; }

        [JsonProperty(Order = 10)]
        public IList<TResource> Items { get; set; }
    }
}