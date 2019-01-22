using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a collection of variables that is attached to a document.
    /// </summary>
    public class VariableSetResource : Resource, IHaveSpaceResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableSetResource" /> class.
        /// </summary>
        public VariableSetResource()
        {
            Variables = new List<VariableResource>();
        }

        /// <summary>
        /// Gets or sets the ID of the document that owns these variables.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the version number.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets the collection of variables.
        /// </summary>
        public IList<VariableResource> Variables { get; set; }

        /// <summary>
        /// Gets the scope values that apply to the variables.
        /// </summary>
        public VariableScopeValues ScopeValues { get; set; }

        public VariableSetResource AddOrUpdateVariableValue(string name, string value)
        {
            var existing = Variables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
                (x.Scope == null || x.Scope.Equals(new ScopeSpecification())));

            if (existing == null)
            {
                var template = new VariableResource
                {
                    Name = name,
                    Value = value,
                };

                Variables.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Value = value;
            }

            return this;
        }

        public VariableSetResource AddOrUpdateVariableValue(string name, string value, string description)
        {
            var existing = Variables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) &&
                                                         (x.Scope == null || x.Scope.Equals(new ScopeSpecification())));

            if (existing == null)
            {
                var template = new VariableResource
                {
                    Name = name,
                    Value = value,
                    Description = description
                };

                Variables.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Value = value;
                existing.Description = description;
            }

            return this;
        }

        public VariableSetResource AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope)
        {
            var existing = Variables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.Scope.Equals(scope));
            if (existing == null)
            {
                var template = new VariableResource
                {
                    Name = name,
                    Value = value,
                    Scope = scope,
                };

                Variables.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Value = value;
                existing.Scope = scope;
            }

            return this;
        }

        public VariableSetResource AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope, bool isSensitive)
        {
            var existing = Variables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.Scope.Equals(scope));
            if (existing == null)
            {
                var template = new VariableResource
                {
                    Name = name,
                    Value = value,
                    Scope = scope,
                    IsSensitive = isSensitive
                };

                Variables.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Value = value;
                existing.Scope = scope;
                existing.IsSensitive = isSensitive;
            }

            return this;
        }

        public VariableSetResource AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope, bool isSensitive, string description)
        {
            var existing = Variables.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase) && x.Scope.Equals(scope));
            if (existing == null)
            {
                var template = new VariableResource
                {
                    Name = name,
                    Value = value,
                    Scope = scope,
                    IsSensitive = isSensitive,
                    Description = description
                };

                Variables.Add(template);
            }
            else
            {
                existing.Name = name;
                existing.Value = value;
                existing.Scope = scope;
                existing.IsSensitive = isSensitive;
                existing.Description = description;
            }

            return this;
        }

        public string SpaceId { get; set; }
    }
}