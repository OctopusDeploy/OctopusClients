using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEnvironmentRepository : IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        List<MachineResource> GetMachines(EnvironmentResource environment);
        void Sort(string[] environmentIdsInOrder);
        EnvironmentEditor CreateOrModify(string name);
        EnvironmentEditor CreateOrModify(string name, string description);
    }
    
    class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
    {
        public EnvironmentRepository(IOctopusClient client)
            : base(client, "Environments")
        {
        }

        public List<MachineResource> GetMachines(EnvironmentResource environment)
        {
            var resources = new List<MachineResource>();

            Client.Paginate<MachineResource>(environment.Link("Machines"), new { }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public void Sort(string[] environmentIdsInOrder)
        {
            Client.Put(Client.RootDocument.Link("EnvironmentSortOrder"), environmentIdsInOrder);
        }

        public EnvironmentEditor CreateOrModify(string name)
        {
            return new EnvironmentEditor(this).CreateOrModify(name);
        }

        public EnvironmentEditor CreateOrModify(string name, string description)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description);
        }
    }
}