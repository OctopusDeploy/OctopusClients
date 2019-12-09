using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class VariableScopeValues : IVariableScopeValues
    {
        public VariableScopeValues()
        {
            Environments = new List<ReferenceDataItem>();
            Machines = new List<ReferenceDataItem>();
            Actions = new List<ReferenceDataItem>();
            Roles = new List<ReferenceDataItem>();
            Channels = new List<ReferenceDataItem>();
            TenantTags = new List<ReferenceDataItem>();
            Processes = new List<ProcessReferenceDataItem>();
        }

        public List<ReferenceDataItem> Environments { get; set; }
        public List<ReferenceDataItem> Machines { get; set; }
        public List<ReferenceDataItem> Actions { get; set; }
        public List<ReferenceDataItem> Roles { get; set; }
        public List<ReferenceDataItem> Channels { get; set; }
        public List<ReferenceDataItem> TenantTags { get; set; }
        public List<ProcessReferenceDataItem> Processes { get; set; }
    }
}