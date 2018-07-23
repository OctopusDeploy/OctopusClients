using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class PackageReference
    {
        string name;
        
        /// <summary>
        /// Constructs a primary package (an un-named package reference)
        /// </summary>
        /// <param name="packageId">The package ID or a variable-expression.</param>
        /// <param name="feedId">The Octopus feed ID or a variable-expression.</param>
        /// <param name="acquisitionLocation">One of <see cref="PackageAcquisitionLocations"/>, or a variable-expression.</param>
        public PackageReference(string packageId, string feedId, string acquisitionLocation)
        :this(null, packageId, feedId, acquisitionLocation)
        {
        }
        
        /// <summary>
        /// Constructs a primary package (an un-named package reference)
        /// </summary>
        /// <param name="packageId">The package ID or a variable-expression.</param>
        /// <param name="feedId">The Octopus feed ID or a variable-expression.</param>
        public PackageReference(string packageId, string feedId)
        :this(packageId, feedId, PackageAcquisitionLocations.Server)
        { }

        /// <summary>
        /// Constructs a named package reference.
        /// </summary>
        /// <param name="name">The package reference name</param>
        /// <param name="packageId">The package ID or a variable-expression.</param>
        /// <param name="feedId">The Octopus feed ID or a variable-expression.</param>
        /// <param name="acquisitionLocation">One of <see cref="PackageAcquisitionLocations"/>, or a variable-expression.</param>
        [JsonConstructor]
        public PackageReference(string name, string packageId, string feedId, string acquisitionLocation)
            :this()
        {
            PackageId = packageId;
            FeedId = feedId;
            AcquisitionLocation = acquisitionLocation;
            this.name = name;
        }

        public PackageReference()
        {
           Properties = new Dictionary<string, string>(); 
        }
        
        /// <summary>
        /// The package reference name.  This may be empty in the case where it is the primary
        /// package for the deployment action. 
        /// </summary>
        public string Name
        {
            get => name ?? "";
            set => name = value;
        }
        
        /// <summary>
        /// The package ID or a variable-expression
        /// </summary>
        public string PackageId { get; set; }
        
        /// <summary>
        /// The Octopus feed ID or a variable-expression
        /// </summary>
        public string FeedId { get; set; }
        
        /// <summary>
        /// May be one of <see cref="PackageAcquisitionLocations"/>, or a variable-expression. 
        /// </summary>
        public string AcquisitionLocation { get; set; }
        
        /// <summary>
        /// Properties specific to the deployment action.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, string> Properties { get; }
        
        [JsonIgnore]
        public bool IsPrimaryPackage => Name == "";
    }
}