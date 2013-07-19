using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using OctopusTools.Client;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly OptionSet options;
        readonly IOctopusSessionFactory client;
        readonly ILog log;
        string serverBaseUrl;
        string user;
        string pass;
        string apiKey;
        bool ignoreSslErrors;
        bool enableDebugging;
        IOctopusSession session;

        protected ApiCommand(IOctopusSessionFactory client, ILog log)
        {
            this.client = client;
            this.log = log;

            options = new OptionSet
            {
                {"server=", "The base URL for your Octopus server - e.g., http://your-octopus/", v => serverBaseUrl = v},
                {"user=", "[Optional] Username to use when authenticating with the server.", v => user = v},
                {"pass=|password=", "[Optional] Password to use when authenticating with the server.", v => pass = v},
                {"apiKey=", "Your API key. Get this from the user profile page.", v => apiKey = v},
                {"configFile=", "[Optional] Text file of default values, with one 'key = value' per line.", v => ReadAdditionalInputsFromConfigurationFile(v)},
                {"debug", "[Optional] Enable debug logging", v => enableDebugging = true},
                {"ignoreSslErrors", "[Optional] Set this flag if your Octopus server uses HTTPS but the certificate is not trusted on this machine. Any certificate errors will be ignored. WARNING: this option may create a security vulnerability.", v => ignoreSslErrors = true},
                {"enableServiceMessages", "[Optional] Enable TeamCity service messages when logging.", v => log.EnableServiceMessages()}
            };
        }

        protected ILog Log
        {
            get { return log; }
        }

        protected IOctopusSession Session
        {
            get
            {
                if (session == null)
                    throw new InvalidOperationException("The session has not yet been initialized.");
                return session;
            }
        }

        protected RootDocument ServiceRoot
        {
            get { return Session.RootDocument; }
        }

        public void GetHelp(TextWriter writer)
        {
            var commandSpecific = new OptionSet();
            SetOptions(commandSpecific);

            if (commandSpecific.Count > 0)
            {
                writer.WriteLine();
                writer.WriteLine("Command arguments:");
                writer.WriteLine();
                commandSpecific.WriteOptionDescriptions(writer);

                writer.WriteLine();
                writer.WriteLine("Common arguments:");
            }

            writer.WriteLine();
            
            options.WriteOptionDescriptions(writer);
        }

        public void Execute(string[] commandLineArguments)
        {
            SetOptions(options);

            var remainingArguments = options.Parse(commandLineArguments);
            if (remainingArguments.Count > 0)
                throw new CommandException("Unrecognized command arguments: " + string.Join(", ", remainingArguments));

            if (string.IsNullOrWhiteSpace(serverBaseUrl))
                throw new CommandException("Please specify the Octopus Server URL using --server=http://your-server/");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new CommandException("Please specify your API key using --apiKey=ABCDEF123456789. Learn more at: https://github.com/OctopusDeploy/Octopus-Tools");

            var credentials = ParseCredentials(user, pass);

            Uri serverBaseUri;
            if (!Uri.TryCreate(serverBaseUrl, UriKind.Absolute, out serverBaseUri)) 
                throw new CommandException("Invalid URI format. Please specify an Octopus Server URL using --server=http://your-server/");
            if (serverBaseUri.Scheme != "http" && serverBaseUri.Scheme != "https")
                throw new CommandException("Invalid URI format, the URI should start with 'http' or 'https'. Please specify an Octopus Server URL using --server=http://your-server/");

            session = client.OpenSession(serverBaseUri, credentials, apiKey, enableDebugging);

            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                {
                    return true;
                }

                var certificate2 = (X509Certificate2)certificate;
                var warning = "The following certificate errors were encountered when establishing the HTTPS connection to the server: " + errors + Environment.NewLine +
                              "Certificate subject name: " + certificate2.SubjectName.Name + Environment.NewLine +
                              "Certificate thumbprint:   " + ((X509Certificate2)certificate).Thumbprint;

                if (ignoreSslErrors)
                {
                    log.Warn(warning);
                    log.Warn("Because --ignoreSslErrors was set, this will be ignored.");
                    return true;
                }

                log.Error(warning);
                return false;
            };

            log.Debug("Handshaking with Octopus server: " + serverBaseUrl);
            var root = session.RootDocument;
            log.Debug("Handshake successful. Octopus version: " + root.Version + "; API version: " + root.ApiVersion);

            Execute();
        }

        protected virtual void SetOptions(OptionSet options)
        {
        }

        protected abstract void Execute();

        static NetworkCredential ParseCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                return CredentialCache.DefaultNetworkCredentials;

            var split = username.Split('\\');
            if (split.Length == 2)
            {
                var domain = split.First();
                username = split.Last();

                return new NetworkCredential(username, password, domain);
            }

            return new NetworkCredential(username, password);
        }

        protected List<string> ReadAdditionalInputsFromConfigurationFile(string configFile)
        {
            configFile = Path.GetFullPath(configFile);

            log.Debug("Loading additional arguments from config file: " + configFile);

            if (!File.Exists(configFile))
            {
                throw new CommandException("Unable to find config file " + configFile);
            }

            var results = File.ReadAllLines(configFile).Select(line => "--" + line.Trim()).ToList();

            var remainingArguments = options.Parse(results);
            if (remainingArguments.Count > 0)
                throw new CommandException("Unrecognized arguments in configuration file: " + string.Join(", ", remainingArguments));

            return results;
        }
    }
}