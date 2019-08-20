using System;
using System.IO;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class TenantEditor : IResourceEditor<TenantResource, TenantEditor>
    {
        private readonly ITenantRepository repository;
        private readonly Lazy<TenantVariablesEditor> variables;

        public TenantEditor(ITenantRepository repository)
        {
            this.repository = repository;
            variables = new Lazy<TenantVariablesEditor>(() => new TenantVariablesEditor(repository, Instance).Load());
        }

        public TenantResource Instance { get; private set; }

        public TenantVariablesEditor Variables => variables.Value;

        public TenantEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new TenantResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;
                Instance = repository.Modify(existing);
            }

            return this;
        }
        
        public TenantEditor CreateOrModify(string name, string description, string cloneId = null)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new TenantResource
                {
                    Name = name,
                    Description = description,
                }, new { clone = cloneId });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;
                Instance = repository.Modify(existing);
            }

            return this;
        }

        public TenantEditor SetLogo(string logoFilePath)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream);
            }

            return this;
        }

        public TenantEditor ClearTags()
        {
            Instance.ClearTags();
            return this;
        }

        public TenantEditor WithTag(TagResource tag)
        {
            Instance.WithTag(tag);
            return this;
        }

        public TenantEditor ClearProjects()
        {
            Instance.ClearProjects();
            return this;
        }

        public TenantEditor ConnectToProjectAndEnvironments(ProjectResource project, params EnvironmentResource[] environments)
        {
            Instance.ConnectToProjectAndEnvironments(project, environments);
            return this;
        }

        public TenantEditor Customize(Action<TenantResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public TenantEditor Save()
        {
            Instance = repository.Modify(Instance);
            if (variables.IsValueCreated)
            {
                variables.Value.Save();
            }
            return this;
        }
    }
}