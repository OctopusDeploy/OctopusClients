using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesAwsAuthenticationResource : KubernetesStandardAccountAuthenticationResource
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
        
        /// <summary>
        /// Whether or not to use the role assigned to the instance this deployment is performed on
        /// </summary>
        public bool UseInstanceRole { get; set; }
        
        /// <summary>
        /// Whether to assume a role
        /// </summary>
        public bool AssumeRole { get; set; }
        
        /// <summary>
        /// The name of the role to assume
        /// </summary>
        public string AssumedRoleArn { get; set; }
        
        /// <summary>
        /// The name of the assume role session
        /// </summary>
        public string AssumedRoleSession { get; set; }
        
        /// <summary>
        /// The duration of the assumed role
        /// </summary>
        public int? AssumeRoleSessionDurationSeconds { get; set; }
        
        /// <summary>
        /// An external id for the assumed role
        /// </summary>
        public string AssumeRoleExternalId { get; set; }
    }
}
