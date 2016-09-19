using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IGetAll<TResource>
    {
        Task<List<TResource>> GetAll();
    }
}