using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// The FeedResource type has been deprecated on Octopus Deploy 3.5 servers. Use NuGetFeedResource instead")]
    /// </summary>
    public class FeedResource : Resource, INamedResource
    {
        [Writeable]
        public string Name { get; set; }

        [WriteableOnCreate]
        public virtual FeedType FeedType { get; }
    }
}