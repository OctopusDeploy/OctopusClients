using System;
using System.Net;

namespace OctopusTools.Client
{
    public interface IOctopusClientFactory
    {
        IOctopusClient Create(string apiKey, Uri serverBaseUrl, ICredentials credentials);
    }
}