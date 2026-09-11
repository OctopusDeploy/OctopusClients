using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IEnvironmentRepository : IFindBySlug<EnvironmentResource>, IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null);
        Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
            CancellationToken cancellationToken,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<EnvironmentsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false);
        Task<EnvironmentsSummaryResource> Summary(
            CancellationToken cancellationToken,
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Sort(string[] environmentIdsInOrder);
        Task Sort(string[] environmentIdsInOrder, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<EnvironmentEditor> CreateOrModify(string name);
        Task<EnvironmentEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<EnvironmentEditor> CreateOrModify(string name, string description);
        Task<EnvironmentEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure);
        Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure, CancellationToken cancellationToken);
    }

    class EnvironmentRepository : BasicRepository<EnvironmentResource>, IEnvironmentRepository
    {
        public EnvironmentRepository(IOctopusAsyncRepository repository)
            : base(repository, "Environments")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null)
            => GetMachines(environment, CancellationToken.None, skip, take, partialName, roles, isDisabled, healthStatuses, commStyles, tenantIds, tenantTags);

        public async Task<List<MachineResource>> GetMachines(EnvironmentResource environment,
            CancellationToken cancellationToken,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null)
        {
            var resources = new List<MachineResource>();

            await Client.Paginate<MachineResource>(environment.Link("Machines"), new
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
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false)
            => Summary(CancellationToken.None, ids, partialName, machinePartialName, roles, isDisabled, healthStatuses, commStyles, tenantIds, tenantTags, hideEmptyEnvironments);

        public async Task<EnvironmentsSummaryResource> Summary(
            CancellationToken cancellationToken,
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            bool? hideEmptyEnvironments = false)
        {
            return await Client.Get<EnvironmentsSummaryResource>(await Repository.Link("EnvironmentsSummary").ConfigureAwait(false), new
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
            }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Sort(string[] environmentIdsInOrder)
        {
            return Sort(environmentIdsInOrder, CancellationToken.None);
        }

        public async Task Sort(string[] environmentIdsInOrder, CancellationToken cancellationToken)
        {
            await Client.Put(await Repository.Link("EnvironmentSortOrder").ConfigureAwait(false), environmentIdsInOrder, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> CreateOrModify(string name)
        {
            return CreateOrModify(name, CancellationToken.None);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new EnvironmentEditor(this).CreateOrModify(name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> CreateOrModify(string name, string description)
        {
            return CreateOrModify(name, description, CancellationToken.None);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure)
        {
            return CreateOrModify(name, description, allowDynamicInfrastructure, CancellationToken.None);
        }

        public Task<EnvironmentEditor> CreateOrModify(string name, string description, bool allowDynamicInfrastructure, CancellationToken cancellationToken)
        {
            return new EnvironmentEditor(this).CreateOrModify(name, description, allowDynamicInfrastructure);
        }
    }
}
