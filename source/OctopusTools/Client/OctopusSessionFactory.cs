using System;
using System.Net;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusSessionFactory : IOctopusSessionFactory
    {
        readonly ILog log;
        string serverBaseUrl;
        string user;
        string pass;

        public OctopusSessionFactory(ILog log, ICommandLineArgsProvider commandLineArgsProvider)
        {
            this.log = log;

            var options = new OptionSet();
            options.Add("server=", "The base URL for your Octopus server - e.g., http://myserver/", v => serverBaseUrl = v);
            options.Add("user=", "[Optional] Username to use when authenticating with the server.", v => user = v);
            options.Add("pass=", "[Optional] Password to use when authenticating with the server.", v => pass = v);

            options.Parse(commandLineArgsProvider.Args);
        }


        public IOctopusSession OpenSession()
        {
            if (string.IsNullOrWhiteSpace(serverBaseUrl))
                throw new CommandException("Please specify a server using the parameter: --server=http://myserver");

            var uri = new Uri(serverBaseUrl);
            uri = uri.EnsureEndsWith("/api");

            var credentials = CredentialCache.DefaultNetworkCredentials;
            if (!string.IsNullOrWhiteSpace(user))
            {
                credentials = new NetworkCredential(user, pass);
            }

            return new OctopusSession(uri, credentials, log); ;
        }
    }
}