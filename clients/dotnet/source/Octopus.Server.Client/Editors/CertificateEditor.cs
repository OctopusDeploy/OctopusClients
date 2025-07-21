using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class CertificateEditor : IResourceEditor<CertificateResource, CertificateEditor>
    {
        private readonly ICertificateRepository repository;

        public CertificateEditor(ICertificateRepository repository)
        {
            this.repository = repository;
        }

        public CertificateResource Instance { get; private set; }

        public CertificateEditor Create(string name, string certificateData)
        {
            var existing = repository.FindByName(name);
            if (existing != null)
            {
                throw new ArgumentException($"A certificate with the name {name} already exists");
            }
            
            Instance = repository.Create(new CertificateResource(name, certificateData));
            
            return this;
        }

        public CertificateEditor FindByName(string name)
        {
            var existing = repository.FindByName(name);
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

        public CertificateEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }

        public CertificateUsageResource Usages()
        {
            return repository.Client.Get<CertificateUsageResource>(Instance.Link("Usages"));
        }
    }
}