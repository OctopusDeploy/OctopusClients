using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Model
{
    public class TenantVariableResourceFixture
    {
        [Test]
        public void SettingProjectVariable_Works()
        {
            var project = new ProjectResource("Projects-1", "TestProject", "test-project");
            var environment = new EnvironmentResource { Id = "Environments-1" };
            var templateId = Guid.NewGuid().ToString();
            var resource = new TenantVariableResourceBuilder()
                .WithProjectEnvironment(project, environment)
                .WithProjectTemplate(project, action => action
                    .WithId(templateId)
                    .WithName("ProjectVariable1"))
                .Build();

            resource.SetProjectVariableValue(project, environment, "ProjectVariable1", "SomeValue");

            resource.ProjectVariables["Projects-1"].Variables["Environments-1"]
                .Should()
                .ContainKey(templateId)
                .WhoseValue.Should().BeEquivalentTo(new PropertyValueResource("SomeValue"));
        }

        [Test]
        public void SettingLibraryVariable_Works()
        {
            var libraryVariableSet = new LibraryVariableSetResource { Id = "LibraryVariableSets-1" };

            var templateId = Guid.NewGuid().ToString();
            const string templateName = "LibraryVariable1";

            var resource = new TenantVariableResourceBuilder()
                .WithLibraryTemplate(libraryVariableSet, action => action
                    .WithId(templateId)
                    .WithName(templateName))
                .Build();

            resource.SetLibraryVariableValue(libraryVariableSet, templateName, "SomeValue");

            resource.LibraryVariables[libraryVariableSet.Id].Variables
                .Should()
                .ContainKey(templateId)
                .WhoseValue.Should().BeEquivalentTo(new PropertyValueResource("SomeValue"));
        }
    }

    public class TenantVariableResourceBuilder
    {
        private Dictionary<string, TenantVariableResource.Project> projects = new Dictionary<string, TenantVariableResource.Project>();
        private Dictionary<string, TenantVariableResource.Library> library = new Dictionary<string, TenantVariableResource.Library>();

        public TenantVariableResourceBuilder WithProjectEnvironment(ProjectResource projectResource, EnvironmentResource environmentResource)
        {
            if (!projects.TryGetValue(projectResource.Id, out var projectVariables))
            {
                projectVariables = projects[projectResource.Id] = new TenantVariableResource.Project(projectResource.Id);
            }

            projectVariables.Variables[environmentResource.Id] = new Dictionary<string, PropertyValueResource>();
            return this;
        }

        public TenantVariableResourceBuilder WithProjectTemplate(ProjectResource projectResource, Action<ActionTemplateParameterResourceBuilder> buildAction)
        {
            if (!projects.TryGetValue(projectResource.Id, out var projectVariables))
            {
                projectVariables = projects[projectResource.Id] = new TenantVariableResource.Project(projectResource.Id);
            }

            var builder = new ActionTemplateParameterResourceBuilder();
            buildAction(builder);
            var resource = builder.Build();
            projectVariables.Templates.Add(resource);

            return this;
        }

        public TenantVariableResourceBuilder WithLibraryTemplate(LibraryVariableSetResource libraryVariableSetResource, Action<ActionTemplateParameterResourceBuilder> buildAction)
        {
            if (!library.TryGetValue(libraryVariableSetResource.Id, out var libraryVariables))
            {
                libraryVariables = library[libraryVariableSetResource.Id] = new TenantVariableResource.Library(libraryVariableSetResource.Id);
            }

            var builder = new ActionTemplateParameterResourceBuilder();
            buildAction(builder);
            var resource = builder.Build();
            libraryVariables.Templates.Add(resource);

            return this;
        }

        public TenantVariableResource Build()
        {
            var resource = new TenantVariableResource
            {
                ProjectVariables = projects,
                LibraryVariables = library
            };

            return resource;
        }
    }

    public class ActionTemplateParameterResourceBuilder
    {
        private string id;
        private string name;
        private string label;
        private string helpText;
        private Dictionary<string, string> displaySettings = new Dictionary<string, string>();

        public ActionTemplateParameterResourceBuilder WithId(string id)
        {
            this.id = id;
            return this;
        }

        public ActionTemplateParameterResourceBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public ActionTemplateParameterResourceBuilder WithLabel(string label)
        {
            this.label = label;
            return this;
        }

        public ActionTemplateParameterResourceBuilder WithHelpText(string helpText)
        {
            this.helpText = helpText;
            return this;
        }

        public ActionTemplateParameterResourceBuilder WithControlType(string controlType)
        {
            displaySettings[ControlType.ControlTypeKey] = controlType;
            return this;
        }

        public ActionTemplateParameterResource Build()
        {
            return new ActionTemplateParameterResource
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = name ?? Guid.NewGuid().ToString(),
                Label = label ?? Guid.NewGuid().ToString(),
                HelpText = helpText,
                DisplaySettings = displaySettings
            };
        }
    }
}