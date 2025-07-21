using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class SshEndpointResource : EndpointResource
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

        /// <summary>
        /// The .NET Core platform for Calamari.  This determines which build of Calamari will be pushed with deployments.
        /// For full .NET framework (which requires Mono to be installed on the SSH target server), this value should be null.
        /// For the available .NET Core platforms see <see cref="CalamariDotNetCorePlatforms"/>
        /// </summary>
        [Trim]
        [Writeable]
        public string DotNetCorePlatform { get; set; }

        public static class CalamariDotNetCorePlatforms
        {
            public static readonly string Linux64 = "linux-x64";
            public static readonly string Osx64 = "osx-x64";
        }
    }
}