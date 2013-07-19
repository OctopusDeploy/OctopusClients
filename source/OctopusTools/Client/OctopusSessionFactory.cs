using System;
using System.Net;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusSessionFactory : IOctopusSessionFactory
    {
        readonly ILog log;

        public OctopusSessionFactory(ILog log)
        {
            this.log = log;
        }

        public IOctopusSession OpenSession(Uri uri, NetworkCredential credential, string apiKey, bool enableDebugging)
        {
            return new OctopusSession(uri, credential, apiKey, log) {EnableDebugging = enableDebugging};
        }
    }
}