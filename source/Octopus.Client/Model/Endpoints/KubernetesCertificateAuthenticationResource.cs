using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    class KubernetesCertificateAuthenticationResource : IEndpointWithMultipleAuthenticationResource
    {
        /// <summary>
        /// The client certificate id
        /// </summary>
        [Trim]
        [Writeable]
        public string ClientCertificate { get; set; }

        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesCertificateAuthentication
        /// </summary>
        public string AuthenticationType => MultipleAccountType.KubernetesCertificate.ToString();
    }
}
