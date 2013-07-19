using System;
using System.Net;

namespace OctopusTools.Client
{
    public interface IOctopusSessionFactory
    {
        IOctopusSession OpenSession(Uri uri, NetworkCredential credential, string apiKey, bool enableDebugging);
    }
}