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
        private readonly IOctopusRepository repository;

        public MachineRoleRepository(IOctopusRepository repository)
        {
            this.repository = repository;
        }

        public List<string> GetAllRoleNames()
        {
            return repository.Client.Get<string[]>(repository.Link("MachineRoles")).ToList();
        }
    }
}