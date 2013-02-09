using System;
using System.Collections.Generic;
using OctopusTools.Model;

namespace OctopusTools.Client
{
    public interface IOctopusSession : IDisposable
    {
        RootDocument RootDocument { get; }
        string QualifyWebLink(string url);
        IList<TResource> List<TResource>(string path);
        IList<TResource> List<TResource>(string path, QueryString queryString);
        TResource Get<TResource>(string path);
        TResource Get<TResource>(string path, QueryString queryString);
        TResource Create<TResource>(string path, TResource resource);
        TResource Update<TResource>(string path, TResource resource);
        void Delete<TResource>(string path);
    }
}
