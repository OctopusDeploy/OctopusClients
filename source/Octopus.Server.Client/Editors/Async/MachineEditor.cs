using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class MachineEditor : IResourceEditor<MachineResource, MachineEditor>
    {
        private readonly IMachineRepository repository;

        public MachineEditor(IMachineRepository repository)
        {
            this.repository = repository;
        }

        public MachineResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles)
            => CreateOrModify(name, endpoint, environments, roles, CancellationToken.None);

        public async Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new MachineResource
                {
                    Name = name,
                    Endpoint = endpoint,
                    EnvironmentIds = new ReferenceCollection(environments.Select(e => e.Id)),
                    Roles = new ReferenceCollection(roles)
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Endpoint = endpoint;
                existing.EnvironmentIds.ReplaceAll(environments.Select(e => e.Id));
                existing.Roles.ReplaceAll(roles);

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation = null)
            => CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags, tenantedDeploymentParticipation, CancellationToken.None);

        public async Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation,
            CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                var resource = new MachineResource
                {
                    Name = name,
                    Endpoint = endpoint,
                    EnvironmentIds = new ReferenceCollection(environments.Select(e => e.Id)),
                    Roles = new ReferenceCollection(roles),
                    TenantIds = new ReferenceCollection(tenants.Select(t => t.Id)),
                    TenantTags = new ReferenceCollection(tenantTags.Select(t => t.CanonicalTagName)),
                };

                if (tenantedDeploymentParticipation.HasValue)
                {
                    resource.TenantedDeploymentParticipation = tenantedDeploymentParticipation.Value;
                }

                Instance = await repository.Create(resource, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Endpoint = endpoint;
                existing.EnvironmentIds.ReplaceAll(environments.Select(e => e.Id));
                existing.Roles.ReplaceAll(roles);
                existing.TenantIds.ReplaceAll(tenants.Select(t => t.Id));
                existing.TenantTags.ReplaceAll(tenantTags.Select(t => t.CanonicalTagName));

                if (tenantedDeploymentParticipation.HasValue)
                {
                    existing.TenantedDeploymentParticipation = tenantedDeploymentParticipation.Value;
                }

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        //public IEnumerable<MachineResource> BuildSamples<TEndpoint>(
        //    this IMachineRepository repo,
        //    int numberOfDeploymentTargets,
        //    EnvironmentResource[] environments,
        //    string[] roles,
        //    Func<int, string> nameBuilder = null,
        //    Action<TEndpoint> customizeEndpoint = null,
        //    Action<MachineResource> customizeDeploymentTarget = null)
        //    where TEndpoint : EndpointResource, new()
        //{
        //    Log.Information("Building {Count} sample {Endpoint}...", numberOfDeploymentTargets, typeof(TEndpoint).Name);

        //    var namer = nameBuilder ?? (i => RandomStringGenerator.Generate(16));
        //    return Enumerable.Range(1, numberOfDeploymentTargets).Select(i => repository.Build(namer(i), environments, roles, customizeEndpoint, customizeDeploymentTarget));
        //}

        //public MachineResource ForTenant(this MachineResource machine, TenantResource tenant)
        //{
        //    machine.TenantIds.Add(tenant.Id);

        //    return machine;
        //}

        //public MachineResource ForTenantsWithTag(this MachineResource machine, TagResource tag)
        //{
        //    machine.TenantTags.Add(tag.CanonicalTagName);

        //    return machine;
        //}

        public MachineEditor Customize(Action<MachineResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineEditor> Save()
            => Save(CancellationToken.None);

        public async Task<MachineEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
