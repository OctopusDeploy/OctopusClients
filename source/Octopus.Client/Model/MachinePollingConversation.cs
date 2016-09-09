using System;

namespace Octopus.Client.Model
{
    public class MachinePollingConversation
    {
        public DateTime StartedAtUtc { get; set; }
        public Guid InitialMessageId { get; set; }
        public string InitialMessageDescription { get; set; }
    }
}