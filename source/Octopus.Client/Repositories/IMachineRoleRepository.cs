using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IMachineRoleRepository
    {
        Task<IReadOnlyList<string>> GetAllRoleNames();
    }
}