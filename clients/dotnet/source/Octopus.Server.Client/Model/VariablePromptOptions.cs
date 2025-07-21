using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class VariablePromptOptions
    {
        public VariablePromptOptions()
        {
            DisplaySettings = new Dictionary<string, string>();
        }
        
        public string Label { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        
        /// <summary>
        /// Display options for the prompted-variable input UI.
        /// Use key "Octopus.ControlType" to set the control-type <see cref="Octopus.Client.Model.ControlType"/>
        /// Use key "Octopus.SelectOptions" to add select-options when control-type == "Select".
        /// </summary>
        public IDictionary<string, string> DisplaySettings { get; set; }

        public VariablePromptOptions Clone()
        {
            return new VariablePromptOptions
            {
                Label = Label,
                Description = Description,
                Required = Required,
                DisplaySettings = DisplaySettings
            };
        }
    }
}