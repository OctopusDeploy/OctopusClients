using System;
using Octopus.TinyTypes;

namespace Octopus.Client.Model
{
    public class PackageAcquisitionLocationResource(string value) : CaseInsensitiveStringTinyType(value)
    {
        /// <summary>
        /// Package is acquired on the Octopus Server
        /// </summary>
        public static PackageAcquisitionLocationResource Server => new("Server");
        
        /// <summary>
        /// Package is acquired on the execution-target, which may be a deployment-target
        /// or a Worker
        /// </summary>
        public static PackageAcquisitionLocationResource ExecutionTarget => new("ExecutionTarget");
          
        /// <summary>
        /// Package is not acquired
        /// </summary>
        public static PackageAcquisitionLocationResource NotAcquired => new("NotAcquired");
    }

    [Obsolete("Replaced by PackageAcquisitionLocationResource, this will be removed in a future version.")]
    public class PackageAcquisitionLocation(string value) : PackageAcquisitionLocationResource(value);
}