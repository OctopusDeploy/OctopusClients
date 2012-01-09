using System;
using System.Net;
using System.Threading.Tasks;

namespace OctopusTools.Client
{
    public interface IHttpClient
    {
        void SetAuthentication(ICredentials credentials);
        Task<TResource> Put<TResource>(Uri uri, TResource resource);
        Task<TResource> Post<TResource>(Uri uri, TResource resource);
        Task<TResource> Get<TResource>(Uri uri);
    }
}
