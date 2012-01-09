using System;
using System.Net;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusClientFactory : IOctopusClientFactory
    {
        readonly ILog log;

        public OctopusClientFactory(ILog log)
        {
            this.log = log;
        }

        public IOctopusClient Create(Uri serverBaseUrl, ICredentials credentials)
        {
            return new OctopusClient(serverBaseUrl, credentials, log);
        }
    }
}