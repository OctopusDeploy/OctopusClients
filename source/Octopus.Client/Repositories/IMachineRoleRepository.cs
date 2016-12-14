using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Repositories
{
    public interface IMachineRoleRepository
    {
        List<string> GetAllRoleNames();
    }
    
    class MachineRoleRepository : IMachineRoleRepository
    {
        readonly IOctopusClient client;

        public MachineRoleRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public List<string> GetAllRoleNames()
        {
            return client.Get<string[]>(client.RootDocument.Link("MachineRoles")).ToList();
        }
    }
}