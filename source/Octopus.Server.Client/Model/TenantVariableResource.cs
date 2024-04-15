using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class TenantVariableResource : Resource, IHaveSpaceResource
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; }

        public Dictionary<string, Project> ProjectVariables { get; set; } = new Dictionary<string, Project>();
        public Dictionary<string, Library> LibraryVariables { get; set; } = new Dictionary<string, Library>();
        
        // Token to validate that no variables for the given tenant have changed since this TenantVariableResource was last requested.
        public string ConcurrencyToken { get; set; }

        public PropertyValueResource GetProjectVariableValue(
            ProjectResource projectResource,
            EnvironmentResource environmentResource,
            string templateName)
        {
            if (!ProjectVariables.TryGetValue(projectResource.Id, out var projectVariables))
            {
                throw new ArgumentException("Supplied project is not connected", nameof(projectResource));
            }

            return projectVariables.GetVariableValue(environmentResource, templateName);
        }

        public void SetProjectVariableValue(
            ProjectResource projectResource,
            EnvironmentResource environmentResource,
            string templateName,
            PropertyValueResource value)
        {
            if (!ProjectVariables.TryGetValue(projectResource.Id, out var projectVariables))
            {
                throw new ArgumentException("Supplied project is not connected", nameof(projectResource));
            }

            projectVariables.SetVariableValue(environmentResource, templateName, value);
        }

        public PropertyValueResource GetLibraryVariableValue(
            LibraryVariableSetResource libraryVariableSet,
            string templateName)
        {
            if (!LibraryVariables.TryGetValue(libraryVariableSet.Id, out var libraryVariables))
            {
                throw new ArgumentException("Supplied library variable set is not connected", nameof(libraryVariableSet));
            }

            return libraryVariables.GetVariableValue(templateName);
        }

        public void SetLibraryVariableValue(
            LibraryVariableSetResource libraryVariableSetResource,
            string templateName,
            PropertyValueResource value)
        {
            if (!LibraryVariables.TryGetValue(libraryVariableSetResource.Id, out var libraryVariables))
            {
                throw new ArgumentException("Supplied library variable set is not connected", nameof(libraryVariableSetResource));
            }

            libraryVariables.SetVariableValue(templateName, value);
        }

        public class Project
        {
            public Project(string projectId)
            {
                ProjectId = projectId;
            }

            public string ProjectId { get; }
            public string ProjectName { get; set; }

            public List<ActionTemplateParameterResource> Templates { get; set; } = new List<ActionTemplateParameterResource>();

            public Dictionary<string, Dictionary<string, PropertyValueResource>> Variables { get; set; } = new Dictionary<string, Dictionary<string, PropertyValueResource>>();

            public LinkCollection Links { get; set; }

            public PropertyValueResource GetVariableValue(EnvironmentResource environment, string templateName)
            {
                if (!Variables.TryGetValue(environment.Id, out var environmentVariables))
                {
                    throw new ArgumentException("Supplied environment is not connected", nameof(environment));
                }

                var templateId = Templates.SingleOrDefault(t => t.Name == templateName)?.Id;
                if (templateId is null)
                {
                    throw new ArgumentException($"No project variable template with name '{templateName}'");
                }

                return environmentVariables[templateId];
            }

            public void SetVariableValue(EnvironmentResource environment, string templateName, PropertyValueResource value)
            {
                if (!Variables.TryGetValue(environment.Id, out var environmentVariables))
                {
                    throw new ArgumentException("Supplied environment is not connected", nameof(environment));
                }

                var templateId = Templates.SingleOrDefault(t => t.Name == templateName)?.Id;
                if (templateId is null)
                {
                    throw new ArgumentException($"No project variable template with name '{templateName}'");
                }

                environmentVariables[templateId] = value;
            }
        }

        public class Library
        {
            public Library(string libraryVariableSetId)
            {
                LibraryVariableSetId = libraryVariableSetId;
            }

            public string LibraryVariableSetId { get;  }
            public string LibraryVariableSetName { get; set; }

            public List<ActionTemplateParameterResource> Templates { get; set; } = new List<ActionTemplateParameterResource>();

            public Dictionary<string, PropertyValueResource> Variables { get; set; } = new Dictionary<string, PropertyValueResource>();

            public LinkCollection Links { get; set; }

            public PropertyValueResource GetVariableValue(string templateName)
            {
                 var templateId = Templates.SingleOrDefault(t => t.Name == templateName)?.Id;
                 if (templateId is null)
                 {
                     throw new ArgumentException($"No project variable template with name '{templateName}'");
                 }

                 return Variables[templateId];
            }

            public void SetVariableValue(string templateName, PropertyValueResource value)
            {
                var templateId = Templates.SingleOrDefault(t => t.Name == templateName)?.Id;
                if (templateId is null)
                {
                    throw new ArgumentException($"No project variable template with name '{templateName}'");
                }

                Variables[templateId] = value;
            }
        }

        public string SpaceId { get; set; }
    }
}