using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEnvironmentRepository : IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        List<MachineResource> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null);
        EnvironmentsSummaryResource Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false);
        void Sort(string[] environmentIdsInOrder);
        EnvironmentEditor CreateOrModify(string name);
        EnvironmentEditor CreateOrModify(string name, string description);
        EnvironmentEditor CreateOrModify(string name, string description, bool allowDynamicInfrastructure);
    }

    class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
    {
        public EnvironmentRepository(IOctopusRepository repository)
            : base(repository, "Environments")
        {
        }

        public List<MachineResource> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null)
        {
            var resources = new List<MachineResource>();

            Client.Paginate<MachineResource>(environment.Link("Machines"), new
            {
                skip,
                take,
                partialName,
                roles,
                isDisabled,
                healthStatuses,
                commStyles,
                tenantIds,
                tenantTags
            }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public EnvironmentsSummaryResource Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false)
        {
            return Client.Get<EnvironmentsSummaryResource>(Repository.Link("EnvironmentsSummary"), new
            {
                ids,
                partialName,
                machinePartialName,
                roles,
                isDisabled,
                healthStatuses,
                commStyles,
                tenantIds,
                tenantTags,
                hideEmptyEnvironments,
            });
        }

        public void Sort(string[] environmentIdsInOrder)
        {
            Client.Put(Repository.Link("EnvironmentSortOrder"), environmentIdsInOrder);
        }

        public EnvironmentEditor CreateOrModify(string name)
        {
            return new EnvironmentEditor(this).CreateOrModify(name);
        }

        public EnvironmentEditor CreateOrModify(string name, string description)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description);
        }

        public EnvironmentEditor CreateOrModify(string name, string description, bool allowDynamicInfrastructure)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description, allowDynamicInfrastructure);
        }
    }
}