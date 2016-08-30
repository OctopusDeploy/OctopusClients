using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    public class VariableTemplateContainerEditor<TContainer> : IVariableTemplateContainerEditor<TContainer>
        where TContainer : class, IVariableTemplateContainer, IVariableTemplateContainerEditor<TContainer>
    {
        private readonly TContainer container;

        public VariableTemplateContainerEditor(TContainer container)
        {
            this.container = container;
        }

        public TContainer AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings)
        {
            var existing = container.Templates.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                var template = new ActionTemplateParameterResource
                {
                    Name = name,
                    Label = label,
                    DisplaySettings = displaySettings
                };

                container.Templates.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Label = label;
                existing.DisplaySettings = displaySettings;
            }

            return container;
        }

        public TContainer AddOrUpdateVariableTemplate(string name, string label, IDictionary<string, string> displaySettings, string defaultValue, string helpText)
        {
            var existing = container.Templates.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing == null)
            {
                var template = new ActionTemplateParameterResource
                {
                    Name = name,
                    Label = label,
                    DisplaySettings = displaySettings,
                    DefaultValue = defaultValue,
                    HelpText = helpText
                };

                container.Templates.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Label = label;
                existing.DisplaySettings = displaySettings;
                existing.DefaultValue = defaultValue;
                existing.HelpText = helpText;
            }

            return container;
        }

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<string, string> SingleLineTextTemplateDisplaySettings = new Dictionary<string, string>
        {
            {"Octopus.ControlType", "SingleLineText"}
        };

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<string, string> MultiLineTextTemplateDisplaySettings = new Dictionary<string, string>
        {
            {"Octopus.ControlType", "MultiLineText"}
        };

        // ReSharper disable once StaticMemberInGenericType
        private static readonly Dictionary<string, string> SensitiveTemplateDisplaySettings = new Dictionary<string, string>
        {
            {"Octopus.ControlType", "Sensitive"}
        };

        private static Dictionary<string, string> BuildSelectTemplateDisplaySettings(IDictionary<string, string> options)
        {
            return new Dictionary<string, string>
            {
                {"Octopus.ControlType", "Select"},
                {"Octopus.SelectOptions", string.Join(Environment.NewLine, options.Select(o => $"{o.Key}|{o.Value}"))}
            };
        }

        public TContainer AddOrUpdateSingleLineTextTemplate(string name, string label)
        {
            return AddOrUpdateVariableTemplate(name, label, SingleLineTextTemplateDisplaySettings);
        }

        public TContainer AddOrUpdateSingleLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return AddOrUpdateVariableTemplate(name, label, SingleLineTextTemplateDisplaySettings, defaultValue, helpText);
        }

        public TContainer AddOrUpdateMultiLineTextTemplate(string name, string label)
        {
            return AddOrUpdateVariableTemplate(name, label, MultiLineTextTemplateDisplaySettings);
        }

        public TContainer AddOrUpdateMultiLineTextTemplate(string name, string label, string defaultValue, string helpText)
        {
            return AddOrUpdateVariableTemplate(name, label, MultiLineTextTemplateDisplaySettings, defaultValue, helpText);
        }

        public TContainer AddOrUpdateSensitiveTemplate(string name, string label)
        {
            return AddOrUpdateVariableTemplate(name, label, SensitiveTemplateDisplaySettings);
        }

        public TContainer AddOrUpdateSensitiveTemplate(string name, string label, string defaultValue, string helpText)
        {
            return AddOrUpdateVariableTemplate(name, label, SensitiveTemplateDisplaySettings, defaultValue, helpText);
        }

        public TContainer AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options)
        {
            return AddOrUpdateVariableTemplate(name, label, BuildSelectTemplateDisplaySettings(options));
        }

        public TContainer AddOrUpdateSelectTemplate(string name, string label, IDictionary<string, string> options, string defaultValue, string helpText)
        {
            return AddOrUpdateVariableTemplate(name, label, BuildSelectTemplateDisplaySettings(options), defaultValue, helpText);
        }

        public TContainer Clear()
        {
            container.Templates.Clear();
            return container;
        }
    }
}