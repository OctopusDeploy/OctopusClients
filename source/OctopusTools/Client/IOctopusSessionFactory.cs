using System;
using System.Net;

namespace OctopusTools.Client
{
    public interface IOctopusSessionFactory
    {
        IOctopusSession Create(Uri serverBaseUrl, ICredentials credentials);
    }
}