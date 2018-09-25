using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IEnvironmentRepository : IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null);
        Task<EnvironmentsSummaryResource> Summary(
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
        Task Sort(string[] environmentIdsInOrder);
        Task<EnvironmentEditor> CreateOrModify(string name);
        Task<EnvironmentEditor> CreateOrModify(string name, string description);
        Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure);
    }

    class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
    {
        public EnvironmentRepository(IOctopusAsyncRepository repository)
            : base(repository, "Environments")
        {
        }

        public async Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
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

            await Client.Paginate<MachineResource>(environment.Link("Machines"), new {
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
            }).ConfigureAwait(false);

            return resources;
        }

        public Task<EnvironmentsSummaryResource> Summary(
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

        public Task Sort(string[] environmentIdsInOrder)
        {
            return Client.Put(Repository.Link("EnvironmentSortOrder"), environmentIdsInOrder);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name)
        {
            return new EnvironmentEditor(this).CreateOrModify(name);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, string description)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description, allowDynamicInfrastructure);
        }
    }
}
