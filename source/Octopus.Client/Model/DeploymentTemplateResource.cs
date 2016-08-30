using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class DeploymentTemplateResource : Resource
    {
        public DeploymentTemplateResource()
        {
            PromoteTo = new List<DeploymentPromotionTarget>();
            TenantPromotions = new List<DeploymentPromomotionTenant>();
        }

        public bool IsDeploymentProcessModified { get; set; }

        public bool IsVariableSetModified { get; set; }

        public bool IsLibraryVariableSetModified { get; set; }

        public string DeploymentNotes { get; set; }

        public List<DeploymentPromotionTarget> PromoteTo { get; set; }

        public List<DeploymentPromomotionTenant> TenantPromotions { get; set; }
    }
}