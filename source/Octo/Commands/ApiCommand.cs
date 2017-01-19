using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using System.Diagnostics;

namespace Octopus.Cli.Commands
{
    public abstract class ApiCommand : ICommand
    {
        readonly IOctopusClientFactory clientFactory;
        readonly IOctopusAsyncRepositoryFactory repositoryFactory;
        string apiKey;
        bool enableDebugging;
        bool ignoreSslErrors;
        string password;
        string username;
        int? timeOut;
        IOctopusAsyncClient client;

        protected ApiCommand(IOctopusClientFactory clientFactory, IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem)
        {
            this.clientFactory = clientFactory;
            this.repositoryFactory = repositoryFactory;
            this.Log = log;
            this.FileSystem = fileSystem;

            var options = Options.For("Common options");
            options.Add("server=", "The base URL for your Octopus server - e.g., http://your-octopus/", v => ServerBaseUrl = v);
            options.Add("apiKey=", "Your API key. Get this from the user profile page.", v => apiKey = v);
            options.Add("user=", "[Optional] Username to use when authenticating with the server.", v => username = v);
            options.Add("pass=", "[Optional] Password to use when authenticating with the server.", v => password = v);
            options.Add("configFile=", "[Optional] Text file of default values, with one 'key = value' per line.", v => ReadAdditionalInputsFromConfigurationFile(v));
            options.Add("debug", "[Optional] Enable debug logging", v => enableDebugging = true);
            options.Add("ignoreSslErrors", "[Optional] Set this flag if your Octopus server uses HTTPS but the certificate is not trusted on this machine. Any certificate errors will be ignored. WARNING: this option may create a security vulnerability.", v => ignoreSslErrors = true);
            options.Add("enableServiceMessages", "[Optional] Enable TeamCity or Team Foundation Build service messages when logging.", v => log.EnableServiceMessages());
            options.Add("timeout=", $"[Optional] Timeout in seconds for network operations. Default is {ApiConstants.DefaultClientRequestTimeout/1000}.", v => timeOut = int.Parse(v));
        }

        protected Options Options { get; } = new Options();

        protected ILogger Log { get; }

        protected string ServerBaseUrl { get; private set; }

        protected IOctopusAsyncRepository Repository { get; private set; }

        protected OctopusRepositoryCommonQueries RepositoryCommonQueries { get; private set; }

        protected IOctopusFileSystem FileSystem { get; }

        public void GetHelp(TextWriter writer)
        {
            Options.WriteOptionDescriptions(writer);
        }

        public async Task Execute(string[] commandLineArguments)
        {
            var remainingArguments = Options.Parse(commandLineArguments);
            if (remainingArguments.Count > 0)
                throw new CommandException("Unrecognized command arguments: " + string.Join(", ", remainingArguments));

            if (string.IsNullOrWhiteSpace(ServerBaseUrl))
                throw new CommandException("Please specify the Octopus Server URL using --server=http://your-server/");

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new CommandException("Please specify your API key using --apiKey=ABCDEF123456789. Learn more at: https://github.com/OctopusDeploy/Octopus-Tools");

            var credentials = ParseCredentials(username, password);

            var endpoint = new OctopusServerEndpoint(ServerBaseUrl, apiKey, credentials);

#if !HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
#endif

            var options = new OctopusClientOptions()
            {
#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
                IgnoreSslErrors = ignoreSslErrors
#endif
            };
            if (timeOut.HasValue)
            {
                options.Timeout = TimeSpan.FromSeconds(timeOut.Value);
            }

            client = await clientFactory.CreateAsyncClient(endpoint, options).ConfigureAwait(false);
            Repository = repositoryFactory.CreateRepository(client);
            RepositoryCommonQueries = new OctopusRepositoryCommonQueries(Repository, Log);

            if (enableDebugging)
            {
                Repository.Client.SendingOctopusRequest += request => Log.Debug("{Method:l} {Uri:l}", request.Method, request.Uri);
            }

            Log.Debug("Handshaking with Octopus server: {Url:l}", ServerBaseUrl);
            var root = Repository.Client.RootDocument;
            Log.Debug("Handshake successful. Octopus version: {Version:l}; API version: {ApiVersion:l}", root.Version, root.ApiVersion);

            var user = await Repository.Users.GetCurrent().ConfigureAwait(false);
            if (user != null)
            {
                Log.Debug("Authenticated as: {Name:l} <{EmailAddress:l}> {IsService:l}", user.DisplayName, user.EmailAddress, user.IsService ? "(a service account)" : "");
            }

            ValidateParameters();
            await Execute().ConfigureAwait(false);
        }


        protected virtual void ValidateParameters() { }

        protected abstract Task Execute();

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

        protected void SetFlagState(string input, ref bool? setter)
        {
            bool tempBool;
            if (bool.TryParse(input, out tempBool))
            {
                setter = tempBool;
            }
        }

        protected List<string> ReadAdditionalInputsFromConfigurationFile(string configFile)
        {
            configFile = FileSystem.GetFullPath(configFile);

            Log.Debug("Loading additional arguments from config file: {ConfigFile:l}", configFile);

            if (!FileSystem.FileExists(configFile))
            {
                throw new CommandException("Unable to find config file " + configFile);
            }

            var results = new List<string>();
            using (var fileStream = FileSystem.OpenFile(configFile, FileAccess.Read))
            using (var file = new StreamReader(fileStream))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"))
                    {
                        results.Add("--" + line.Trim());
                    }
                }
            }

            var remainingArguments = Options.Parse(results);
            if (remainingArguments.Count > 0)
                throw new CommandException("Unrecognized arguments in configuration file: " + string.Join(", ", remainingArguments));

            return results;
        }

        protected string GetPortalUrl(string path)
        {
            if (!path.StartsWith("/")) path = '/' + path;
            var uri = new Uri(ServerBaseUrl + path);
            return uri.AbsoluteUri;
        }

        protected static IEnumerable<string> FormatReleasePropertiesAsStrings(ReleaseResource release)
        {
            return new List<string>
            {
                "Version: " + release.Version,
                "Assembled: " + release.Assembled,
                "Package Versions: " + GetPackageVersionsAsString(release.SelectedPackages),
                "Release Notes: " + ((release.ReleaseNotes != null) ? release.ReleaseNotes.Replace(Environment.NewLine, @"\n") : "")
            };
        }

        protected static string GetPackageVersionsAsString(IEnumerable<SelectedPackage> packages)
        {
            var packageVersionsAsString = "";

            foreach (var package in packages)
            {
                var packageVersionAsString = package.StepName + " " + package.Version;

                if (packageVersionsAsString.Contains(packageVersionAsString))
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(packageVersionsAsString))
                {
                    packageVersionsAsString += "; ";
                }
                packageVersionsAsString += packageVersionAsString;
            }
            return packageVersionsAsString;
        }


#if !HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;

            var certificate2 = (X509Certificate2)certificate;
            var warning = "The following certificate errors were encountered when establishing the HTTPS connection to the server: " + errors + Environment.NewLine +
                             "Certificate subject name: " + certificate2.SubjectName.Name + Environment.NewLine +
                             "Certificate thumbprint:   " + ((X509Certificate2)certificate).Thumbprint;

            if (ignoreSslErrors)
            {
                Log.Warning(warning);
                Log.Warning("Because --ignoreSslErrors was set, this will be ignored.");
                return true;
            }

            Log.Error(warning);
            return false;
        }
#endif
    }
}