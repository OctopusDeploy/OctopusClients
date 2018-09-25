using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Model;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using System.Diagnostics;
using Serilog;
using Serilog.Events;

namespace Octopus.Cli.Commands
{
    public abstract class ApiCommand : CommandBase, ICommand
    {
        /// <summary>
        /// The environment variable that can hold the Octopus server
        /// </summary>
        public const string ServerUrlEnvVar = "OCTOPUS_CLI_SERVER";
        /// <summary>
        /// The environment variable that can hold the API key
        /// </summary>
        public const string ApiKeyEnvVar = "OCTOPUS_CLI_API_KEY";
        /// <summary>
        /// The environment variable that can hold the username
        /// </summary>
        public const string UsernameEnvVar = "OCTOPUS_CLI_USERNAME";
        /// <summary>
        /// The environment variable that can hold the password
        /// </summary>   
        public const string PasswordEnvVar = "OCTOPUS_CLI_PASSWORD";
        readonly IOctopusClientFactory clientFactory;
        readonly IOctopusAsyncRepositoryFactory repositoryFactory;
        string apiKey;
        string serverBaseUrl;
        bool enableDebugging;
        bool ignoreSslErrors;
        
        string password;
        string username;
        readonly OctopusClientOptions clientOptions = new OctopusClientOptions();

        protected ApiCommand(IOctopusClientFactory clientFactory, IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, ICommandOutputProvider commandOutputProvider) : base(commandOutputProvider)
        {
            this.clientFactory = clientFactory;
            this.repositoryFactory = repositoryFactory;
            this.FileSystem = fileSystem;

            var options = Options.For("Common options");
            options.Add("server=", $"[Optional] The base URL for your Octopus server - e.g., http://your-octopus/. This URL can also be set in the {ServerUrlEnvVar} environment variable.", v => serverBaseUrl = v);
            options.Add("apiKey=", $"[Optional] Your API key. Get this from the user profile page. Your must provide an apiKey or username and password. If the guest account is enabled, a key of API-GUEST can be used. This key can also be set in the {ApiKeyEnvVar} environment variable.", v => apiKey = v);
            options.Add("user=", $"[Optional] Username to use when authenticating with the server. Your must provide an apiKey or username and password. This Username can also be set in the {UsernameEnvVar} environment variable.", v => username = v);
            options.Add("pass=", $"[Optional] Password to use when authenticating with the server. This Password can also be set in the {PasswordEnvVar} environment variable.", v => password = v);
            
            options.Add("configFile=", "[Optional] Text file of default values, with one 'key = value' per line.", v => ReadAdditionalInputsFromConfigurationFile(v));
            options.Add("debug", "[Optional] Enable debug logging", v => enableDebugging = true);
            options.Add("ignoreSslErrors", "[Optional] Set this flag if your Octopus server uses HTTPS but the certificate is not trusted on this machine. Any certificate errors will be ignored. WARNING: this option may create a security vulnerability.", v => ignoreSslErrors = true);
            options.Add("enableServiceMessages", "[Optional] Enable TeamCity or Team Foundation Build service messages when logging.", v => commandOutputProvider.EnableServiceMessages());
            options.Add("timeout=", $"[Optional] Timeout in seconds for network operations. Default is {ApiConstants.DefaultClientRequestTimeout/1000}.", v => clientOptions.Timeout = TimeSpan.FromSeconds(int.Parse(v)));
            options.Add("proxy=", $"[Optional] The URI of the proxy to use, eg http://example.com:8080.", v => clientOptions.Proxy = v);
            options.Add("proxyUser=", $"[Optional] The username for the proxy.", v => clientOptions.ProxyUsername = v);
            options.Add("proxyPass=", $"[Optional] The password for the proxy. If both the username and password are omitted and proxyAddress is specified, the default credentials are used. ", v => clientOptions.ProxyPassword = v);
            options.AddLogLevelOptions();
        }

        protected ILogger Log { get; }

        protected string ServerBaseUrl => string.IsNullOrWhiteSpace(serverBaseUrl)
                    ? System.Environment.GetEnvironmentVariable(ServerUrlEnvVar)
                    : serverBaseUrl;
            
        string ApiKey => string.IsNullOrWhiteSpace(apiKey)
            ? System.Environment.GetEnvironmentVariable(ApiKeyEnvVar)
            : apiKey;
        
        string Username => string.IsNullOrWhiteSpace(username)
            ? System.Environment.GetEnvironmentVariable(UsernameEnvVar)
            : username;
        
        string Password => string.IsNullOrWhiteSpace(password)
            ? System.Environment.GetEnvironmentVariable(PasswordEnvVar)
            : password;

        protected IOctopusAsyncRepository Repository { get; private set; }

        protected OctopusRepositoryCommonQueries RepositoryCommonQueries { get; private set; }

        protected IOctopusFileSystem FileSystem { get; }

        public async Task Execute(string[] commandLineArguments)
        {
            var remainingArguments = Options.Parse(commandLineArguments);

            if (printHelp)
            {
                this.GetHelp(Console.Out, commandLineArguments);
                
                return;
            }

            if (remainingArguments.Count > 0)
                throw new CommandException("Unrecognized command arguments: " + string.Join(", ", remainingArguments));

            if (string.IsNullOrWhiteSpace(ServerBaseUrl))
                throw new CommandException("Please specify the Octopus Server URL using --server=http://your-server/. " +
                    $"The Octopus Server URL can also be set in the {ServerUrlEnvVar} environment variable.");

            if (!string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(Username))
                throw new CommandException("Please provide an API Key OR a username and password, not both. " +
                                           "These values may have been passed in as command line arguments, or may have been set in the " +
                                           $"{ApiKeyEnvVar} and {UsernameEnvVar} environment variables.");

            if (string.IsNullOrWhiteSpace(ApiKey) && string.IsNullOrWhiteSpace(Username))
                throw new CommandException("Please specify your API key using --apiKey=ABCDEF123456789 OR a username and password. " +
                                           $"The API key can also be set in the {ApiKeyEnvVar} environment variable, " +
                                           $"while the username and password can be set in the {UsernameEnvVar} and {PasswordEnvVar} " +
                                           "environment variables respectively. Learn more at: https://github.com/OctopusDeploy/Octopus-Tools");

            var endpoint = string.IsNullOrWhiteSpace(ApiKey)
                ? new OctopusServerEndpoint(ServerBaseUrl)
                : new OctopusServerEndpoint(ServerBaseUrl, ApiKey);

#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
            clientOptions.IgnoreSslErrors = ignoreSslErrors;
#else
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
#endif
            
            commandOutputProvider.PrintMessages = OutputFormat == OutputFormat.Default || enableDebugging;
            commandOutputProvider.PrintHeader();

            var client = await clientFactory.CreateAsyncClient(endpoint, clientOptions).ConfigureAwait(false);
            Repository = await repositoryFactory.CreateRepository(client);
            RepositoryCommonQueries = new OctopusRepositoryCommonQueries(Repository, commandOutputProvider);
            
            if (enableDebugging)
            {
                Repository.Client.SendingOctopusRequest += request => commandOutputProvider.Debug("{Method:l} {Uri:l}", request.Method, request.Uri);
            }

            commandOutputProvider.Debug("Handshaking with Octopus server: {Url:l}", ServerBaseUrl);

            var root = Repository.Client.RootDocument;

            commandOutputProvider.Debug("Handshake successful. Octopus version: {Version:l}; API version: {ApiVersion:l}", root.Version, root.ApiVersion);

            if (!string.IsNullOrWhiteSpace(Username))
            {
                await Repository.Users.SignIn(Username, Password);
            }

            var user = await Repository.Users.GetCurrent().ConfigureAwait(false);
            if (user != null)
            {
                commandOutputProvider.Debug("Authenticated as: {Name:l} <{EmailAddress:l}> {IsService:l}", user.DisplayName, user.EmailAddress, user.IsService ? "(a service account)" : "");
            }

            ValidateParameters();
            await Execute().ConfigureAwait(false);
        }

        protected virtual void ValidateParameters() { }

        protected virtual async Task Execute()
        {
            if (formattedOutputInstance != null)
            {
                await formattedOutputInstance.Request();

                Respond();
            }
            else
            {
                throw new Exception("Need to override the Execute method or implement the ISuportFormattedOutput interface");
            }
        }

        private void Respond()
        {
            if (formattedOutputInstance != null)
            {
                if (OutputFormat == OutputFormat.Json)
                {
                    formattedOutputInstance.PrintJsonOutput();
                }
                else
                {
                    formattedOutputInstance.PrintDefaultOutput();
                }
            }
        }
        
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

            commandOutputProvider.Debug("Loading additional arguments from config file: {ConfigFile:l}", configFile);

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
                "Release Notes: " + GetReleaseNotes(release)
            };
        }

        protected static string GetReleaseNotes(ReleaseResource release)
        {
            return release.ReleaseNotes != null ? release.ReleaseNotes.Replace(System.Environment.NewLine, @"\n") : "";
        }

        protected static string GetPackageVersionsAsString(IEnumerable<SelectedPackage> packages)
        {
            var packageVersionsAsString = "";

            foreach (var package in packages)
            {
                var packageVersionAsString = package.ActionName + " " + package.Version;

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
            var warning = "The following certificate errors were encountered when establishing the HTTPS connection to the server: " + errors + System.Environment.NewLine +
                             "Certificate subject name: " + certificate2.SubjectName.Name + System.Environment.NewLine +
                             "Certificate thumbprint:   " + ((X509Certificate2)certificate).Thumbprint;

            if (ignoreSslErrors)
            {
                commandOutputProvider.Warning(warning);
                commandOutputProvider.Warning("Because --ignoreSslErrors was set, this will be ignored.");
                return true;
            }

            commandOutputProvider.Error(warning);
            return false;
        }
#endif
    }
}