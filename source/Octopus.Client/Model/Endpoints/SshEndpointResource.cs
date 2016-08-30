using System;

namespace Octopus.Client.Model.Endpoints
{
    public class SshEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.Ssh; }
        }

        [Trim]
        [Writeable]
        public string AccountId { get; set; }

        [Trim]
        [Writeable]
        public string Host { get; set; }

        [Writeable]
        public int Port { get; set; }

        [Trim]
        [Writeable]
        public string Fingerprint { get; set; }

        public string Uri
        {
            get
            {
                var uri = new UriBuilder("ssh", Host, Port);
                return uri.ToString();
            }
        }

        [Writeable]
        public string ProxyId { get; set; }
    }
}