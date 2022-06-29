using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a collection of variables that will be resolved for a given deployment scenario
    /// </summary>
    public class VariableSetPreviewResource : VariableSetResource
    {
        public List<string> UnresolvedValues { get; set; }
    }
}