using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class KubernetesStandardAccountAuthenticationResource : IEndpointWithMultipleAuthenticationResource
    {
        /// <summary>
        /// The account id
        /// </summary>
        [Trim]
        [Writeable]
        public string AccountId { get; set; }
        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesStandardAccountAuthentication
        /// </summary>
        public virtual string AuthenticationType => MultipleAccountType.KubernetesStandard.ToString();
    }
}
