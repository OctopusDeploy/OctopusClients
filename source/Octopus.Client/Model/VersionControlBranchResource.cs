using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class VersionControlBranchResource
    {
        public VersionControlBranchResource(string name)
        {
            Name = name;
            Links = new LinkCollection();
        }

        public string Name { get; }

        public LinkCollection Links { get; }

        public void WithLinks(LinkCollection links)
        {
            Links.Clear();
            foreach (var link in links)
            {
                Links.Add(link.Key, link.Value);
            }
        }

        public string Link(string name)
        {
            if (!(Links ?? new LinkCollection()).TryGetValue(name, out var value))
            {
                throw new Exception($"The document does not define a link for '{name}'");
            }

            return value;
        }
    }
}