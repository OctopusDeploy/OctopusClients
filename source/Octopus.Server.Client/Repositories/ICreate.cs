using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICreate<TResource>
    {
        TResource Create(TResource resource, object pathParameters = null);
    }

    public interface ICreateProjectScoped<TResource>
    {
        TResource Create(ProjectResource projectResource, TResource resource, object pathParameter = null);
    }
}