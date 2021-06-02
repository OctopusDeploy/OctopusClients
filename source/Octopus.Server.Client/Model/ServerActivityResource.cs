using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ServerActivityResource : Resource
    {
        public string Description { get; set; }
        public string ActorName { get; set; }
        public string Location { get; set; }
        public int InputQueueCount { get; set; }
        public string CurrentOperation { get; set; }
        public string CurrentlyReceiving { get; set; }
        public DateTime? CurrentlyReceivingSinceUtc { get; set; }
        public List<ServerActivityResource> SupervisedActivities { get; set; }
    }
}