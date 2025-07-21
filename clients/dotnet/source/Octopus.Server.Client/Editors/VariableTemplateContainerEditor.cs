using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Editors
{
    public class VariableTemplateContainerEditor<TContainer>
        where TContainer : IVariableTemplateContainer, IVariableTemplateContainerEditor<TContainer>
    {
        private readonly IVariableTemplateContainerEditor<TContainer> container;

        public VariableTemplateContainerEditor(IVariableTemplateContainerEditor<TContainer> container)
        {
            this.container = container;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings)
        {
            container.AddOrUpdateVariableTemplate(name, label, displaySettings);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings, string defaultValue, string helpText)
        {
            container.AddOrUpdateVariableTemplate(name, label, displaySettings, defaultValue, helpText);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSingleLineTextTemplate(string name, string label)
        {
            container.AddOrUpdateSingleLineTextTemplate(name, label);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSingleLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            container.AddOrUpdateSingleLineTextTemplate(name, label, defaultValue, helpText);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateMultiLineTextTemplate(string name, string label)
        {
            container.AddOrUpdateMultiLineTextTemplate(name, label);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateMultiLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            container.AddOrUpdateMultiLineTextTemplate(name, label, defaultValue, helpText);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSensitiveTemplate(string name, string label)
        {
            container.AddOrUpdateSensitiveTemplate(name, label);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSensitiveTemplate(string name, string label, string defaultValue, string helpText)
        {
            container.AddOrUpdateSensitiveTemplate(name, label, defaultValue, helpText);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options)
        {
            container.AddOrUpdateSelectTemplate(name, label, options);
            return this;
        }

        public VariableTemplateContainerEditor<TContainer> AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options, string defaultValue, string helpText)
        {
            container.AddOrUpdateSelectTemplate(name, label, options, defaultValue, helpText);
            return this;
        }
    }
}