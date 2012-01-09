using System;
using System.Net;

namespace OctopusTools.Client
{
    public interface IOctopusClientFactory
    {
        IOctopusClient Create(Uri serverBaseUrl, ICredentials credentials);
    }
}