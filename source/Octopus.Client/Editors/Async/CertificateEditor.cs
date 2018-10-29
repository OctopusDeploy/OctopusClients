using System;
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

        public async Task<CertificateEditor> Create(string name, string certificateData)
        {
            var existing = repository.FindByName(name);
            if (existing != null)
            {
                throw new ArgumentException($"A certificate with the name {name} already exists");
            }
            
            Instance = await repository.Create(new CertificateResource(name, certificateData)).ConfigureAwait(false);
            
            return this;
        }

        public async Task<CertificateEditor> FindByName(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
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

        public async Task<CertificateEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }

        public async Task<CertificateUsageResource> Usages()
        {
            return await repository.Client.Get<CertificateUsageResource>(Instance.Link("Usages")).ConfigureAwait(false);
        }
    }
}