using System.Collections.Generic;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class PackageReference
    {
        string name;
        
        /// <summary>
        /// Constructs a primary package (an unnamed package reference)
        /// </summary>
        public PackageReference(string packageId, string feedId, string acquisitionLocation)
        :this(null, packageId, feedId, acquisitionLocation)
        {
        }
        
        /// <summary>
        /// Constructs a primary package (an unnamed package reference)
        /// </summary>
        public PackageReference(string packageId, string feedId)
        :this(packageId, feedId, PackageAcquisitionLocations.Server)
        { }

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
        
        public string Name
        {
            get => name ?? "";
            set => name = value;
        }
        
        public string PackageId { get; set; }
        
        public string FeedId { get; set; }
        
        public string AcquisitionLocation { get; set; }
        
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, string> Properties { get; }
        
        [JsonIgnore]
        public bool IsPrimaryPackage => Name == "";
    }
}