using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model.Forms
{
    /// <summary>
    /// A form is a set of form elements, and the values that apply or may be provided for those elements.
    /// </summary>
    public class Form
    {
        [JsonConstructor]
        public Form()
        {
            Elements = new List<FormElement>();
            Values = new Dictionary<string, string>();
        }

        public Form(IEnumerable<FormElement> elements = null, IDictionary<string, string> values = null)
        {
            Values = values != null
                ? new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Elements = elements != null ? elements.ToList() : new List<FormElement>();
        }

        /// <summary>
        /// Values supplied for the form elements.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public Dictionary<string, string> Values { get; private set; }

        /// <summary>
        /// Elements of the form.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public List<FormElement> Elements { get; private set; }
    }
}