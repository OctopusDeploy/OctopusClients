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
        string apiKey;
        string serverBaseUrl;

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
                options.Add("apiKey=", "The API Key from the Settings->API page in your Octopus server.", v => apiKey = v);
                options.Add("server=", "The base URL for your Octopus server - e.g., http://myserver/", v => serverBaseUrl = v);
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
            // TODO: Enable this when the feature is implemented in Octopus
            //if (string.IsNullOrWhiteSpace(apiKey))
            //    throw new CommandException("Please specify an API key using the parameter: --apiKey=XYZ");

            if (string.IsNullOrWhiteSpace(serverBaseUrl))
                throw new CommandException("Please specify a server using the parameter: --server=http://myserver");

            var uri = new Uri(serverBaseUrl);
            uri = uri.EnsureEndsWith("/api");

            return clientFactory.Create(apiKey, uri, CredentialCache.DefaultNetworkCredentials);
        }
    }
}