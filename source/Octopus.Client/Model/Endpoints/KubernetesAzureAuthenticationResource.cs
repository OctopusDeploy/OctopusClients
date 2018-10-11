using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    class KubernetesAzureAuthenticationResource : KubernetesStandardAccountAuthenticationResource
    {
        /// <summary>
        /// The name of the K8S cluster
        /// </summary>
        [Trim]
        [Writeable]
        public string ClusterName { get; set; }

        /// <summary>
        /// The resource group containing the K8S cluster
        /// </summary>
        [Trim]
        [Writeable]
        public string ClusterResourceGroup { get; set; }

        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesAzureAuthentication
        /// </summary>
        public override string AuthenticationType => MultipleAccountType.KubernetesAzure.ToString();
    }
}
