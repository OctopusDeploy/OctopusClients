using System;
using System.Net;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusClientFactory : IOctopusClientFactory
    {
        readonly IHttpClient httpClient;
        readonly ILog log;

        public OctopusClientFactory(IHttpClient httpClient, ILog log)
        {
            this.httpClient = httpClient;
            this.log = log;
        }

        public IOctopusClient Create(Uri serverBaseUrl, ICredentials credentials)
        {
            httpClient.SetAuthentication(credentials);
            return new OctopusClient(httpClient, serverBaseUrl, log);
        }
    }
}