namespace Octopus.Client.Model.Endpoints
{
    /// <summary>
    /// Holds the details of any pod service account used to authenticate with Kubernetes
    /// </summary>
    public class KubernetesPodServiceAuthenticationResource : IEndpointWithMultipleAuthenticationResource
    {
        /// <summary>
        /// The token file path
        /// </summary>
        public string TokenPath { get; set; }

        /// <summary>
        /// Set the "Type" field to this to ensure nevermore deserialises a
        /// KubernetesPodService
        /// </summary>
        public string AuthenticationType => MultipleAccountType.KubernetesPodService.ToString();
    }
}