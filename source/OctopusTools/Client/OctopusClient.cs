using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Client
{
    public class OctopusClient : IOctopusClient
    {
        readonly IHttpClient httpClient;
        readonly Uri serverBaseUri;
        readonly ILog log;

        public OctopusClient(IHttpClient httpClient, Uri serverBaseUri, ILog log)
        {
            this.httpClient = httpClient;
            this.serverBaseUri = serverBaseUri;
            this.log = log;
        }

        public Task<OctopusInstance> Handshake()
        {
            return new Task<OctopusInstance>(() =>
            {
                log.Debug("Handshaking with Octopus server: " + serverBaseUri);
                var task = httpClient.Get<OctopusInstance>(serverBaseUri);
                task.RunSynchronously();
                log.Debug("Handshake successful. Octopus version: " + task.Result.Version);
                return task.Result;
            });
        }

        public Task<IList<TResource>> List<TResource>(string path)
        {
            var uri = new Uri(serverBaseUri, path);

            return httpClient.Get<IList<TResource>>(uri);
        }

        public Task<TResource> Get<TResource>(string path)
        {
            return httpClient.Get<TResource>(QualifyUri(path));
        }

        public Task<TResource> Create<TResource>(string path, TResource resource)
        {
            return httpClient.Post(QualifyUri(path), resource);
        }

        public Task<TResource> Update<TResource>(string path, TResource resource)
        {
            return httpClient.Put(QualifyUri(path), resource);
        }

        Uri QualifyUri(string path)
        {
            return new Uri(serverBaseUri, path);
        }
    }
}
