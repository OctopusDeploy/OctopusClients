using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents a reference from a deployment-process (specifically an action or action-template) to a package.
    /// May be named or un-named.
    /// </summary>
    /// <history>
    /// Prior to Octopus 2018.8, deployment actions could have at most a single package-reference. This was
    /// captured as properties on the action (Octopus.Action.Package.PackageId, Octopus.Action.Package.FeedId, etc).
    /// In 2018.8, we introduced support for actions with multiple packages (initially script steps and kubernetes step).
    /// Storing collections of nested objects in the property-bag gets very messy, so packge-references were moved into their own class
    /// and collection on the deployment actions.
    /// </history>
    public class PackageReference 
    {
        string name;
        
        /// <summary>
        /// Constructs a named package-reference. 
        /// </summary>
        /// <param name="name">The package-reference name.</param>
        /// <param name="packageId">The package ID or a variable-expression</param>
        /// <param name="feedId">The feed ID or a variable-expression</param>
        /// <param name="acquisitionLocation">The location the package should be acquired</param>
        public PackageReference(string name, string packageId, string feedId, PackageAcquisitionLocation acquisitionLocation)
            :this(name, packageId, feedId, acquisitionLocation.ToString())
        {
        }
        
        /// <summary>
        /// Constructs a named package-reference. 
        /// </summary>
        /// <param name="name">The package-reference name.</param>
        /// <param name="packageId">The package ID or a variable-expression</param>
        /// <param name="feedId">The feed ID or a variable-expression</param>
        /// <param name="acquisitionLocation">The location the package should be acquired.
        /// May be one <see cref="PackageAcquisitionLocation"/> or a variable-expression.</param>
        [JsonConstructor]
        public PackageReference(string name, string packageId, string feedId, string acquisitionLocation)
            :this()
        {
            PackageId = packageId;
            FeedId = feedId;
            AcquisitionLocation = acquisitionLocation;
            this.name = name;
        }
        
        /// <summary>
        /// Constructs a primary package (an un-named package reference)
        /// </summary>
        public PackageReference(string packageId, string feedId, PackageAcquisitionLocation acquisitionLocation)
        :this(null, packageId, feedId, acquisitionLocation)
        {
        }
        
        /// <summary>
        /// Constructs a primary package (an un-named package reference)
        /// </summary>
        public PackageReference(string packageId, string feedId, string acquisitionLocation)
        :this(null, packageId, feedId, acquisitionLocation)
        {
        }
        
        /// <summary>
        /// Constructs a primary package (an un-named package reference)
        /// </summary>
        public PackageReference(string packageId, string feedId)
        :this(packageId, feedId, PackageAcquisitionLocation.Server)
        { }

        public PackageReference()
        {
           Properties = new Dictionary<string, string>(); 
        }
        
        /// <summary>
        /// An name for the package-reference.
        /// This may be empty.
        /// This is used to discriminate the package-references. Package ID isn't suitable because an action may potentially
        /// have multiple references to the same package ID (e.g. if you wanted to use different versions of the same package).
        /// Also, the package ID may be a variable-expression. 
        /// </summary>
        public string Name
        {
            get => name ?? "";
            set => name = value;
        }
        
        /// <summary>
        /// Package ID or a variable-expression 
        /// </summary>
        public string PackageId { get; set; }
        
        /// <summary>
        /// Feed ID or a variable-expression
        /// </summary>
        public string FeedId { get; set; }
        
        /// <summary>
        /// The package-acquisition location.
        /// One of <see cref="PackageAcquisitionLocation"/> or a variable-expression 
        /// </summary>
        public string AcquisitionLocation { get; set; } 
        
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, string> Properties { get; private set; }

        [JsonIgnore]
        public bool IsPrimaryPackage => Name == "";

        public PackageReference Clone()
        {
            return new PackageReference(name, PackageId, FeedId, AcquisitionLocation)
            {
                Properties = new Dictionary<string, string>(Properties)
            };
        }
    }
}
