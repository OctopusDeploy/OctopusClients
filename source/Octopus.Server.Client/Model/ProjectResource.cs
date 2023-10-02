using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility.Attributes;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Validation;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a project.
    /// </summary>
    public class ProjectResource : ResourceWithExtensionSettings, INamedResource, IVariableTemplateContainer, IVariableTemplateContainerEditor<ProjectResource>, IHaveSpaceResource, IHaveSlugResource
    {
        private readonly IVariableTemplateContainerEditor<ProjectResource> variableTemplateEditor;

        public ProjectResource()
        {
#pragma warning disable 618
            IncludedLibraryVariableSetIds = new List<string>();
#pragma warning restore 618
            Templates = new List<ActionTemplateParameterResource>();
#pragma warning disable 618
            ProjectConnectivityPolicy = new DeploymentConnectivityPolicy();
#pragma warning restore 618
            AutoDeployReleaseOverrides = new HashSet<AutoDeployReleaseOverrideResource>(AutoDeployReleaseOverrideResource.EnvironmentIdTenantIdComparer);
            variableTemplateEditor = new VariableTemplateContainerEditor<ProjectResource>(this);
            PersistenceSettings = new DatabasePersistenceSettingsResource();
        }

        public ProjectResource(string id, string name, string slug) : this()
        {
            Id = id;
            Name = name;
            Slug = slug;
        }

        [Writeable]
        [JsonProperty(Order = 2)]
        [ContainsSomeValidCharacters]
        public string Name { get; set; }

        [JsonProperty(Order = 3)]
        public string Slug { get; set; }

        [Writeable]
        [JsonProperty(Order = 20)]
        public string Description { get; set; }

        [Writeable]
        [JsonProperty(Order = 22)]
        public bool IsDisabled { get; set; }

        [Writeable]
        [JsonProperty(Order = 23)]
        public string ProjectGroupId { get; set; }

        public string VariableSetId { get; set; }
        public string DeploymentProcessId { get; set; }
        public string ClonedFromProjectId { get; set; }

        [Writeable]
        [JsonProperty(Order = 24)]
        public string LifecycleId { get; set; }

        [Writeable]
        [JsonProperty(Order = 25)]
        public bool AutoCreateRelease { get; set; }
        [Writeable]
        [JsonProperty(Order = 26)]
        public bool IsVersionControlled { get; set; }
        [JsonProperty(Order = 27)]
        public PersistenceSettingsResource PersistenceSettings { get; set; }

        /// <summary>
        /// Treats releases of different channels to the same environment as a 
        /// separate deployment dimension. 'False' indicates a "hotfix"-style 
        /// usage of channels (single release active per environment ignoring channels), 
        /// whereas `True` indicates "microservice"-style usage (single release per environment per channel)
        /// </summary>
        [Writeable]
        public bool DiscreteChannelRelease { get; set; }

        /// <summary>
        /// Library variable sets included in the project. Sets are listed in order
        /// of precedence, with earlier items in the list overriding any variables
        /// with the same name and scope definition appearing later in the list.
        /// </summary>
        [Writeable]
        public List<string> IncludedLibraryVariableSetIds { get; set; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public bool DefaultToSkipIfAlreadyInstalled { get; set; }

        [Writeable]
        public TenantedDeploymentMode TenantedDeploymentMode { get; set; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public GuidedFailureMode DefaultGuidedFailureMode { get; set; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public VersioningStrategyResource VersioningStrategy { get; set; }

        [Writeable]
        public ReleaseCreationStrategyResource ReleaseCreationStrategy { get; set; }

        public List<ActionTemplateParameterResource> Templates { get; set; }

        [Writeable]
        [JsonProperty(Order = 45, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public DeploymentConnectivityPolicy ProjectConnectivityPolicy { get; set; }

        [Writeable]
        public ISet<AutoDeployReleaseOverrideResource> AutoDeployReleaseOverrides { get; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public string ReleaseNotesTemplate { get; set; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public string DeploymentChangesTemplate { get; set; }

        [Writeable]
        [Obsolete("Use " + nameof(DeploymentSettingsResource) + " instead on the `deploymentsettings` API.")]
        public bool ForcePackageDownload { get; set; }
        
        public IconResource Icon { get; set; }

        public ProjectResource Clear()
        {
            return variableTemplateEditor.Clear();
        }

        public ProjectResource AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateVariableTemplate(name, label, displaySettings, defaultValue, helpText);
        }

        public ProjectResource AddOrUpdateSingleLineTextTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateSingleLineTextTemplate(name, label);
        }

        public ProjectResource AddOrUpdateSingleLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSingleLineTextTemplate(name, label, defaultValue, helpText);
        }

        public ProjectResource AddOrUpdateMultiLineTextTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateMultiLineTextTemplate(name, label);
        }

        public ProjectResource AddOrUpdateMultiLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateMultiLineTextTemplate(name, label, defaultValue, helpText);
        }

        public ProjectResource AddOrUpdateSensitiveTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateSensitiveTemplate(name, label);
        }

        public ProjectResource AddOrUpdateSensitiveTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSensitiveTemplate(name, label, defaultValue, helpText);
        }

        public ProjectResource AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options)
        {
            return variableTemplateEditor.AddOrUpdateSelectTemplate(name, label, options);
        }

        public ProjectResource AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSelectTemplate(name, label, options, defaultValue, helpText);
        }

        public ProjectResource IncludingLibraryVariableSets(params LibraryVariableSetResource[] libraryVariableSets)
        {
            if (libraryVariableSets == null) throw new ArgumentNullException(nameof(libraryVariableSets));

            foreach (var set in libraryVariableSets)
            {
#pragma warning disable 618
                if (!IncludedLibraryVariableSetIds.Contains(set.Id))
#pragma warning restore 618
                {
#pragma warning disable 618
                    IncludedLibraryVariableSetIds.Add(set.Id);
#pragma warning restore 618
                }
            }

            return this;
        }

        public ProjectResource AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings)
        {
            var existing = Templates.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                var template = new ActionTemplateParameterResource
                {
                    Name = name,
                    Label = label,
                    DisplaySettings = displaySettings
                };

                Templates.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Label = label;
                existing.DisplaySettings = displaySettings;
            }

            return this;
        }

        public void AddAutoDeployReleaseOverride(EnvironmentResource environment, ReleaseResource release)
        {
            AddAutoDeployReleaseOverride(environment, null, release);
        }

        public void AddAutoDeployReleaseOverride(EnvironmentResource environment, TenantResource tenant, ReleaseResource release)
        {
            var autoDeployReleaseOverrideResource = new AutoDeployReleaseOverrideResource(environment.Id, tenant?.Id, release.Id);

            var existingAutoDeployReleaseOverride = AutoDeployReleaseOverrides.SingleOrDefault(x => x.EnvironmentId == environment.Id && x.TenantId == tenant?.Id);
            if (existingAutoDeployReleaseOverride != null)
                AutoDeployReleaseOverrides.Remove(existingAutoDeployReleaseOverride);

            AutoDeployReleaseOverrides.Add(autoDeployReleaseOverrideResource);
        }

        public string SpaceId { get; set; }
    }
}