using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class TenantsMissingVariablesResource
    {
        public TenantsMissingVariablesResource(string tenantId)
        {
            TenantId = tenantId;
            Links = new LinkCollection();
        }

        public string TenantId { get; }

        public LinkCollection Links { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<MissingVariableResource> MissingVariables { get; set; }
    }

    public class MissingVariableResource
    {
        public MissingVariableResource()
        {
            Links = new LinkCollection();
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ProjectId { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LibraryVariableSetId { get; set; }

        public string VariableTemplateName { get; set; }
        public string VariableTemplateId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EnvironmentId { get; set; }

        public LinkCollection Links { get; set; }
    }
}
