using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// The FeedResource type has been deprecated on Octopus Deploy 3.5 servers. Use NuGetFeedResource instead")]
    /// </summary>
    public class FeedResource : Resource, INamedResource, IHaveSpaceResource
    {
        [Writeable]
        public string Name { get; set; }

        [WriteableOnCreate]
        public virtual FeedType FeedType { get; }

        public string SpaceId { get; set; }

        /// <summary>
        /// The package-acquisition locations supported by the feed.
        /// The first is considered the default.
        /// </summary>
        public IList<PackageAcquisitionLocation> PackageAcquisitionLocationOptions { get; } = new List<PackageAcquisitionLocation>();
    }
}