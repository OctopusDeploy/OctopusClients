using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Cli.Model
{
    public class OctopusRuleTestResponse : Resource
    {
        public IEnumerable<string> Errors { get; set; }
        public bool SatisfiesVersionRange { get; set; }
        public bool SatisfiesPreReleaseTag { get; set; }
    }
}
