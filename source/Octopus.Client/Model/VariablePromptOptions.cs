using System;

namespace Octopus.Client.Model
{
    public class VariablePromptOptions
    {
        public string Label { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }

        public VariablePromptOptions Clone()
        {
            return new VariablePromptOptions
            {
                Label = Label,
                Description = Description,
                Required = Required
            };
        }
    }
}