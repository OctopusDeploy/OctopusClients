using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    class KubernetesAwsAuthenticationResource : KubernetesStandardAccountAuthenticationResource
    {
        /// <summary>
        /// The name of the EKS cluster
        /// </summary>
        [Trim]
        [Writeable]
        public string ClusterName { get; set; }

        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesAwsAuthentication
        /// </summary>
        public override string AuthenticationType => MultipleAccountType.KubernetesAws.ToString();
    }
}
