using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    public class DashboardTenantResource : Resource
    {
        public string Name { get; set; }
        public IDictionary<string, ReferenceCollection> ProjectEnvironments { get; set; }
        public ReferenceCollection TenantTags { get; set; }
        public bool IsDisabled { get; set; }
    }
}
