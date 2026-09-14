using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class CertificateEditor : IResourceEditor<CertificateResource, CertificateEditor>
    {
        private readonly ICertificateRepository repository;

        public CertificateEditor(ICertificateRepository repository)
        {
            this.repository = repository;
        }

        public CertificateResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateEditor> Create(string name, string certificateData)
            => Create(name, certificateData, CancellationToken.None);

        public async Task<CertificateEditor> Create(string name, string certificateData, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing != null)
            {
                throw new ArgumentException($"A certificate with the name {name} already exists");
            }

            Instance = await repository.Create(new CertificateResource(name, certificateData), cancellationToken).ConfigureAwait(false);

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateEditor> FindByName(string name)
            => FindByName(name, CancellationToken.None);

        public async Task<CertificateEditor> FindByName(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                throw new ArgumentException($"A certificate with the name {name} could not be found");
            }
            else
            {
                Instance = existing;
            }

            return this;
        }

        public CertificateEditor Customize(Action<CertificateResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateEditor> Save()
            => Save(CancellationToken.None);

        public async Task<CertificateEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateUsageResource> Usages()
            => Usages(CancellationToken.None);

        public async Task<CertificateUsageResource> Usages(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<CertificateUsageResource>(Instance.Link("Usages"), cancellationToken).ConfigureAwait(false);
        }
    }
}
