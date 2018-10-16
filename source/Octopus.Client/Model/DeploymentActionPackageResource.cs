using System;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DeploymentActionPackageResource
    {
        /// <param name="deploymentAction">The name (or id) of the deployment action from the deployment process</param>
        public DeploymentActionPackageResource(string deploymentAction)
            : this(deploymentAction, "")
        { }
        
        /// <param name="deploymentAction">The name (or id) of the deployment action from the deployment process</param>
        /// <param name="packageReference">The name (or id) of the package-reference. This can be null or empty if it is the primary (un-named) package reference on an action.</param>
        [JsonConstructor]
        public DeploymentActionPackageResource(string deploymentAction, string packageReference)
        {
            DeploymentAction = deploymentAction;
            PackageReference = packageReference;
        }
        
        /// <summary>
        /// The name of the deployment-action containing the package reference.
        /// </summary>
        public string DeploymentAction { get; set; }
        
        /// <summary>
        /// The name of the package-reference.  This can be an empty string in the case of the 
        /// </summary>
        public string PackageReference { get; set; }

        /// <summary>
        /// Performs a case-insensitive comparison
        /// </summary>
        public bool DeploymentActionNameMatches(string name)
        {
            return DeploymentAction.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Performs a case-insensitive comparison between the name of this package-reference and the
        /// supplied name.  Nulls and empty strings are considered equal. 
        /// </summary>
        public bool PackageReferenceNameMatches(string name)
        {
            return string.IsNullOrEmpty(PackageReference)  
                ? string.IsNullOrEmpty(name) 
                : PackageReference.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        internal string ToLegacyStringFormat()
        {
            return !string.IsNullOrEmpty(PackageReference)
                ? $"{DeploymentAction}:{PackageReference}"
                : DeploymentAction;
        }

        internal static DeploymentActionPackageResource FromLegacyStringFormat(string s)
        {
            var split = s.Split(':');
            return new DeploymentActionPackageResource(split[0], split.ElementAtOrDefault(1));
        }
    }
}