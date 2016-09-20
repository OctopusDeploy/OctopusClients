using System;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IMachineRoleRepository
    {
        List<string> GetAllRoleNames();
    }
}