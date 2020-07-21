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

        /// <remarks>
        /// TODO: We may want to derive an IVcsControlledResource at some point that this will be a part of
        /// </remarks>
        public LinkCollection Links { get; }

        public void WithLinks(LinkCollection links)
        {
            Links.Clear();
            foreach (var link in links)
            {
                Links.Add(link.Key, link.Value);
            }
        }
    }
}