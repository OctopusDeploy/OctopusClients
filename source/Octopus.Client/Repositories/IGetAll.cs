using System;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IGetAll<TResource>
    {
        List<TResource> GetAll();
    }
}