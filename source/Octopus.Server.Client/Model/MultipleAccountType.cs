using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model
{
    /// <summary>
    /// The different styles of authentication supported by Kubernetes
    /// </summary>
    public enum MultipleAccountType
    {
        None,
        KubernetesStandard,
        KubernetesAws,
        KubernetesAzure,
        KubernetesCertificate,
        KubernetesPodService,
        KubernetesGoogleCloud
    }
}
