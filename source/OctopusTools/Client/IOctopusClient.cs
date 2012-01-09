using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OctopusTools.Model;

namespace OctopusTools.Client
{
    public interface IOctopusClient
    {
        Task<OctopusInstance> Handshake();
        Task<IList<TResource>> List<TResource>(string path);
        Task<TResource> Get<TResource>(string path);
        Task<TResource> Create<TResource>(string path, TResource resource);
        Task<TResource> Update<TResource>(string path, TResource resource);
    }
}
