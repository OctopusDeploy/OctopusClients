using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class FeedResource : Resource, INamedResource
    {
        /// <summary>
        /// The FeedResource type has been deprecated on Octopus Deploy 3.5 servers. Use NuGetFeedResource instead")]
        /// </summary>
        public FeedResource()
        {
            Password = new SensitiveValue();
        }

        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string FeedUri { get; set; }

        [Writeable]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; }

        [WriteableOnCreate]
        public virtual FeedType FeedType { get; }
        
        /// <summary>
        /// The package-acquisition locations supported by the feed.
        /// The first is considered the default.
        /// </summary>
        public IList<PackageAcquisitionLocation> PackageAcquisitionLocationOptions { get; } = new List<PackageAcquisitionLocation>();
    }
}