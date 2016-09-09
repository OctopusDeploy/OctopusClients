using System;
using Octopus.Client.Model;

namespace Octopus.Client.Editors
{
    public interface IResourceEditor<out TResource, out TResourceBuilder> : IResourceBuilder
        where TResource : Resource
        where TResourceBuilder : IResourceBuilder
    {
        TResource Instance { get; }
        TResourceBuilder Customize(Action<TResource> customize);
        TResourceBuilder Save();
    }

    public interface IResourceBuilder
    {
        
    }
}