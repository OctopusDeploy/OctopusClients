using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Serialization
{
    public class EndpointWithMultipleAuthenticationConverter : InheritedClassConverter<IEndpointWithMultipleAuthenticationResource, MultipleAccountType>
    {
        static readonly IDictionary<MultipleAccountType, Type> FeedTypeMappings =
            new Dictionary<MultipleAccountType, Type>
            {
                {MultipleAccountType.KubernetesAzure, typeof(KubernetesAzureAuthenticationResource)},
                {MultipleAccountType.KubernetesAws, typeof(KubernetesAwsAuthenticationResource)},
                {MultipleAccountType.KubernetesCertificate, typeof(KubernetesCertificateAuthenticationResource)},
                {MultipleAccountType.KubernetesStandard, typeof(KubernetesStandardAccountAuthenticationResource)},
            };

        protected override IDictionary<MultipleAccountType, Type> DerivedTypeMappings => FeedTypeMappings;
        protected override string TypeDesignatingPropertyName => nameof(IEndpointWithMultipleAuthenticationResource.AuthenticationType);
    }
}