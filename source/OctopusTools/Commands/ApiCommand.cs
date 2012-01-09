using System;
using System.Net;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly IOctopusClientFactory clientFactory;
        readonly Lazy<IOctopusClient> client;
        readonly ILog log;
        string serverBaseUrl;
        string user;
        string pass;

        protected ApiCommand(IOctopusClientFactory clientFactory, ILog log)
        {
            this.clientFactory = clientFactory;
            this.log = log;

            client = new Lazy<IOctopusClient>(CreateAndInitializeClient);
        }

        public virtual OptionSet Options
        {
            get 
            {
                var options = new OptionSet();
                options.Add("server=", "The base URL for your Octopus server - e.g., http://myserver/", v => serverBaseUrl = v);
                options.Add("user=", "[Optional] Username to use when authenticating with the server.", v => user = v);
                options.Add("pass=", "[Optional] Password to use when authenticating with the server.", v => pass = v);
                return options;
            }
        }

        protected ILog Log
        {
            get { return log; }
        }

        protected IOctopusClient Client
        {
            get { return client.Value; }
        }

        public abstract void Execute();

        IOctopusClient CreateAndInitializeClient()
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

            return clientFactory.Create(uri, credentials);
        }
    }
}