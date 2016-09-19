using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.DeploymentProcess;
using Octopus.Client.Model;

namespace Octopus.Client.Editors
{
    public interface IResourceEditor<out TResource, TResourceBuilder> : IResourceBuilder
        where TResource : Resource
        where TResourceBuilder : IResourceBuilder
    {
        TResource Instance { get; }
        TResourceBuilder Customize(Action<TResource> customize);
        Task<TResourceBuilder> Save();
    }

    public interface IResourceBuilder
    {
        
    }
}