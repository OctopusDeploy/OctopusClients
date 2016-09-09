using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class DeploymentPromomotionTenant : Resource
    {
        public DeploymentPromomotionTenant()
        {
            PromoteTo = new List<DeploymentPromotionTarget>();
        }

        public string Name { get; set; }
        public List<DeploymentPromotionTarget> PromoteTo { get; set; }
    }
}