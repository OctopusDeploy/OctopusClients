using System;

namespace Octopus.Client.Model
{
    public class VariableResource
    {
        public VariableResource()
        {
            Id = Guid.NewGuid().ToString();
            Scope = new ScopeSpecification();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public ScopeSpecification Scope { get; set; }
        public bool IsSensitive { get; set; }
        public bool IsEditable { get; set; }
        public VariablePromptOptions Prompt { get; set; }
    }
}