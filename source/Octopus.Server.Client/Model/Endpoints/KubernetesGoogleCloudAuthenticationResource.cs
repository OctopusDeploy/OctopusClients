namespace Octopus.Client.Model.Endpoints
{
    /// <summary>
    /// Holds the details of any google cloud account used to authenticate with Kubernetes
    /// </summary>
    public class KubernetesGoogleCloudAuthenticationResource : KubernetesStandardAccountAuthenticationResource
    {
        /// <summary>
        /// The name of the GKE cluster
        /// </summary>
        public string ClusterName { get; set; }
        /// <summary>
        /// Whether or not to use the VM service account assigned to the instance this deployment is performed on
        /// </summary>
        public bool UseVmServiceAccount { get; set; }
        /// <summary>
        /// Whether or not to impersonate another service account on the instance this deployment is performed on
        /// </summary>
        public bool ImpersonateServiceAccount { get; set; }
        /// <summary>
        /// The service account emails to impersonate
        /// </summary>
        public string ServiceAccountEmails { get; set; }
        /// <summary>
        /// The project Id of the GCP instance
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// The region code of the GCP instance
        /// </summary>
        public string Region { get; set; }
        /// <summary>
        /// The zone code of the GCP instance
        /// </summary>
        public string Zone { get; set; }
        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesGoogleCloud
        /// </summary>
        public override string AuthenticationType => MultipleAccountType.KubernetesGoogleCloud.ToString();
    }
}