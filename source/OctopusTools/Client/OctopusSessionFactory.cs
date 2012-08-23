using System;
using System.Linq;
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
        string apiKey;

        public OctopusSessionFactory(ILog log, ICommandLineArgsProvider commandLineArgsProvider)
        {
            this.log = log;

            var options = new OptionSet();
            options.Add("server=", "The base URL for your Octopus server - e.g., http://myserver/", v => serverBaseUrl = v);
            options.Add("user=", "[Optional] Username to use when authenticating with the server.", v => user = v);
            options.Add("pass=", "[Optional] Password to use when authenticating with the server.", v => pass = v);
            options.Add("apiKey=", "Your API key.", v => apiKey = v);

            options.Parse(commandLineArgsProvider.Args);
        }

        public IOctopusSession OpenSession()
        {
            if (string.IsNullOrWhiteSpace(serverBaseUrl))
                throw new CommandException("Please specify a server using the parameter: --server=http://myserver");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new CommandException("Please specify an API key. You can get your API key from the Octopus user page. Example: --apiKey=ABCDEF123456789");

            var uri = new Uri(serverBaseUrl);
            uri = uri.EnsureEndsWith("/api");

            var credentials = ParseCredentials();

            return new OctopusSession(uri, credentials, apiKey, log); ;
        }

        NetworkCredential ParseCredentials()
        {
            if (string.IsNullOrWhiteSpace(user))
                return CredentialCache.DefaultNetworkCredentials;

            var split = user.Split('\\');
            if (split.Length == 2)
            {
                var domain = split.First();
                user = split.Last();
                
                return new NetworkCredential(user, pass, domain);
            }

            return new NetworkCredential(user, pass);
        }
    }
}