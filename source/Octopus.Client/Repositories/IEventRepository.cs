using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEventRepository : IGet<EventResource>
    {
        ResourceCollection<EventResource> List(int skip = 0,
                string from = null,
                string to = null,
                string regarding = null,
                string regardingAny = null,
                string users = null,
                string projects = null,
                string environments = null,
                string eventGroups = null,
                string eventCategories = null,
                string tenants = null,
                string tags = null);
    }
}