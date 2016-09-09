using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    public class TenantProjectEnvironmentMappingResource : Resource
    {
        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public ReferenceCollection Environments { get; set; }
    }
}
