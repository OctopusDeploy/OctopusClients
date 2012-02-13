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

        public IOctopusSession OpenSession(Uri serverBaseUrl, ICredentials credentials)
        {
            return new OctopusSession(serverBaseUrl, credentials, log);
        }
    }
}