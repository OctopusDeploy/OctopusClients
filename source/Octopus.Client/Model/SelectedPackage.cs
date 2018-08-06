using System;

namespace Octopus.Client.Model
{
    public class SelectedPackage
    {
        public SelectedPackage()
        {
        }

        /// <param name="actionName">The name of the deployment action</param>
        /// <param name="version">The selected package version</param>
        public SelectedPackage(string actionName, string version)
         : this(actionName, null, version)
        {
        }

        /// <param name="actionName">The name of the deployment action</param>
        /// <param name="packageReferenceName">The name of the package reference on the action.
        /// <see cref="PackageReference.Name"/></param>
        /// <param name="version">The selected package version</param>
        public SelectedPackage(string actionName, string packageReferenceName, string version)
        {
            ActionName = actionName;
            PackageReferenceName = packageReferenceName ?? "";
            Version = version;
        }

        [Obsolete("Replaced by " + nameof(ActionName))]
        public string StepName
        {
            get => ActionName;
            set => ActionName = value;
        }

        /// <summary>
        /// The name of the deployment action
        /// </summary>
        public string ActionName { get; set; }
        
        /// <summary>
        /// The name of the package reference <see cref="PackageReference.Name"/> 
        /// </summary>
        /// <remarks>May be empty for steps which have a primary package</remarks>
        public string PackageReferenceName { 
            get; 
        }
        
        /// <summary>
        ///  The selected package version
        /// </summary>
        public string Version { get; set; }
    }
}