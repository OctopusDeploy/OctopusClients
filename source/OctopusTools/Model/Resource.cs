using System;
using System.Collections.Generic;
using System.Linq;

namespace OctopusTools.Model
{
    public class Resource
    {
        public Dictionary<string, string> Links { get; set; }

        public string Link(string name)
        {
            var links = Links.Select(kvp => new { kvp.Key, kvp.Value });

            var link = links.FirstOrDefault(l => string.Equals(l.Key, name, StringComparison.InvariantCultureIgnoreCase));
            if (link == null)
            {
                throw new Exception(string.Format("The document does not define a link for '{0}'", name));
            }

            return link.Value;
        }
    }
}