using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    public class ConfigurationItemResource : Resource
    {
        public ConfigurationItemResource() : base()
        {
        }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
