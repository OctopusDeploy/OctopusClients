using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    public class FeaturesConfigurationResource : Resource
    {
        [Writeable]
        public bool IsMultiTenancyEnabled { get; set; }

        [Writeable]
        public bool IsDockerEnabled { get; set; }
    }
}
