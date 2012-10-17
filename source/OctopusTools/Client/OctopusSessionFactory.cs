using System;
using System.IO;
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
        protected string serverBaseUrl;
        protected string user;
        protected string pass;
        protected string apiKey;
        string configFile;

        public OctopusSessionFactory(ILog log, ICommandLineArgsProvider commandLineArgsProvider)
        {
            this.log = log;

            var options = new OptionSet();
            options.Add("server=", "The base URL for your Octopus server - e.g., http://myserver/", v => serverBaseUrl = v);
            options.Add("user=", "[Optional] Username to use when authenticating with the server.", v => user = v);
            options.Add("pass=", "[Optional] Password to use when authenticating with the server.", v => pass = v);
            options.Add("apiKey=", "Your API key.", v => apiKey = v);
            options.Add("configFile=", "[Optional] Text file of default values, with one 'key = value' per line.", v => configFile = v);

            options.Parse(commandLineArgsProvider.Args);

            SetDefaultValuesFromConfigFile();
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

        protected void SetDefaultValuesFromConfigFile()
        {
            if(configFile == null)
            {
                return;
            }

            if (!File.Exists(configFile))
            {
                throw new CommandException("Unable to find config file "+configFile);
            }

            string line = null;
            var file = new StreamReader(configFile);
            while((line = file.ReadLine()) != null)
            {
                var idxOfDelimiter = line.IndexOf('=');
                if(idxOfDelimiter == -1 || idxOfDelimiter == line.Length-1)
                {
                    continue;
                }

                var key = line.Substring(0, idxOfDelimiter).Trim();
                var value = line.Substring(idxOfDelimiter+1).Trim();

                if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (serverBaseUrl == null && String.Equals("server", key, StringComparison.InvariantCultureIgnoreCase)) serverBaseUrl = value;
                if (apiKey == null && String.Equals("apiKey", key, StringComparison.InvariantCultureIgnoreCase)) apiKey = value;
                if (user == null && String.Equals("user", key, StringComparison.InvariantCultureIgnoreCase)) user = value;
                if (pass == null && String.Equals("pass", key, StringComparison.InvariantCultureIgnoreCase)) pass = value;

            }
            file.Close();
        }
    }
}