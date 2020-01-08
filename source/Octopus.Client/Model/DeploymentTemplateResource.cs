using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class DeploymentTemplateResource : DeploymentTemplateBaseResource
    {
        public bool IsDeploymentProcessModified { get; set; }

        public string DeploymentNotes { get; set; }
    }

    public class DeploymentTemplateBaseResource : Resource
    {
        public DeploymentTemplateBaseResource()
        {
            PromoteTo = new List<DeploymentPromotionTarget>();
            TenantPromotions = new List<DeploymentPromomotionTenant>();
        }

        public bool IsVariableSetModified { get; set; }

        public bool IsLibraryVariableSetModified { get; set; }

        public List<DeploymentPromotionTarget> PromoteTo { get; set; }

        public List<DeploymentPromomotionTenant> TenantPromotions { get; set; }
    }
}