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
        public VariableType Type { get; set; }
        public ScopeSpecification Scope { get; set; }
        public bool IsEditable { get; set; }
        public VariablePromptOptions Prompt { get; set; }

        /// <summary>
        /// This property is kept for backwards compatibility. 
        /// New way is to use Type = VariableType.Sensitive.
        /// </summary>
        public bool IsSensitive { get; set; }
    }
}