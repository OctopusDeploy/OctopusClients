using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// A standalone variable set that can be included in projects where required.
    /// </summary>
    public class LibraryVariableSetResource : Resource, INamedResource, IVariableTemplateContainer, IVariableTemplateContainerEditor<LibraryVariableSetResource>, IHaveSpaceResource
    {
        private readonly IVariableTemplateContainerEditor<LibraryVariableSetResource> variableTemplateEditor;

        public LibraryVariableSetResource()
        {
            Templates = new List<ActionTemplateParameterResource>();
            variableTemplateEditor = new VariableTemplateContainerEditor<LibraryVariableSetResource>(this);
        }

        /// <summary>
        /// Gets or sets the name of this variable set. This should be short, preferably 5-20 characters.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a description of this variable set that explains the purpose of
        /// the variable set to other users. This field may contain markdown.
        /// </summary>
        [Writeable]
        [Trim]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the id of the associated variable set.
        /// </summary>
        public string VariableSetId { get; set; }

        /// <summary>
        /// Describes the purpose of the variable set. Clients can use this to offer an editing experience
        /// appropriately.
        /// </summary>
        [WriteableOnCreate]
        public VariableSetContentType ContentType { get; set; }
        
        /// <summary>
        /// Gets the variable templates.
        /// </summary>
        public List<ActionTemplateParameterResource> Templates { get; set; }

        public LibraryVariableSetResource Clear()
        {
            return variableTemplateEditor.Clear();
        }

        public LibraryVariableSetResource AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings)
        {
            return variableTemplateEditor.AddOrUpdateVariableTemplate(name, label, displaySettings);
        }

        public LibraryVariableSetResource AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateVariableTemplate(name, label, displaySettings, defaultValue, helpText);
        }

        public LibraryVariableSetResource AddOrUpdateSingleLineTextTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateSingleLineTextTemplate(name, label);
        }

        public LibraryVariableSetResource AddOrUpdateSingleLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSingleLineTextTemplate(name, label, defaultValue, helpText);
        }

        public LibraryVariableSetResource AddOrUpdateMultiLineTextTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateMultiLineTextTemplate(name, label);
        }

        public LibraryVariableSetResource AddOrUpdateMultiLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateMultiLineTextTemplate(name, label, defaultValue, helpText);
        }

        public LibraryVariableSetResource AddOrUpdateSensitiveTemplate(string name, string label)
        {
            return variableTemplateEditor.AddOrUpdateSensitiveTemplate(name, label);
        }

        public LibraryVariableSetResource AddOrUpdateSensitiveTemplate(string name, string label, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSensitiveTemplate(name, label, defaultValue, helpText);
        }

        public LibraryVariableSetResource AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options)
        {
            return variableTemplateEditor.AddOrUpdateSelectTemplate(name, label, options);
        }

        public LibraryVariableSetResource AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options, string defaultValue, string helpText)
        {
            return variableTemplateEditor.AddOrUpdateSelectTemplate(name, label, options, defaultValue, helpText);
        }

        public string SpaceId { get; set; }
    }
}