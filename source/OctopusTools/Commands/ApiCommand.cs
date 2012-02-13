using System;
using System.Net;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly IOctopusSessionFactory sessionFactory;
        readonly Lazy<IOctopusSession> client;
        readonly ILog log;
        string serverBaseUrl;
        string user;
        string pass;

        protected ApiCommand(IOctopusSessionFactory sessionFactory, ILog log)
        {
            this.sessionFactory = sessionFactory;
            this.log = log;

            client = new Lazy<IOctopusSession>(CreateAndInitializeClient);
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

        protected IOctopusSession Session
        {
            get { return client.Value; }
        }

        protected RootDocument ServiceRoot
        {
            get { return client.Value.RootDocument; }
        }

        public abstract void Execute();

        IOctopusSession CreateAndInitializeClient()
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

            return sessionFactory.OpenSession(uri, credentials);
        }
    }
}