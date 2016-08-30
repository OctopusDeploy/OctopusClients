using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public interface IVariableTemplateContainerEditor<out TContainer>
        where TContainer : IVariableTemplateContainer, IVariableTemplateContainerEditor<TContainer>
    {
        TContainer Clear();
        TContainer AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings);
        TContainer AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings, string defaultValue, string helpText);
        TContainer AddOrUpdateSingleLineTextTemplate(string name, string label);
        TContainer AddOrUpdateSingleLineTextTemplate(string name, string label, string defaultValue, string helpText);
        TContainer AddOrUpdateMultiLineTextTemplate(string name, string label);
        TContainer AddOrUpdateMultiLineTextTemplate(string name, string label, string defaultValue, string helpText);
        TContainer AddOrUpdateSensitiveTemplate(string name, string label);
        TContainer AddOrUpdateSensitiveTemplate(string name, string label, string defaultValue, string helpText);
        TContainer AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options);
        TContainer AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options, string defaultValue, string helpText);
    }
}