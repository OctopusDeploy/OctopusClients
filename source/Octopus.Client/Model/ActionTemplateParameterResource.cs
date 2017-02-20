using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ActionTemplateParameterResource
    {
        public ActionTemplateParameterResource()
        {
            DisplaySettings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Id { get; set; }

        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        [Trim]
        public string Label { get; set; }

        [Writeable]
        public string HelpText { get; set; }

        [Writeable]
        public PropertyValueResource DefaultValue { get; set; }

        [Writeable]
        public IDictionary<string, string> DisplaySettings { get; set; }
    }
}