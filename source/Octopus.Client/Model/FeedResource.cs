using System;

namespace Octopus.Client.Model
{
    public class FeedResource : Resource, INamedResource
    {
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
    }
}