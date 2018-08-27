namespace Octopus.Client.Model
{
    public enum PackageAcquisitionLocation
    {
        /// <summary>
        /// Package is acquired on the Octopus Server
        /// </summary>
        Server,
        
        /// <summary>
        /// Package is acquired on the execution-target, which may be a deployment-target
        /// or a Worker
        /// </summary>
        ExecutionTarget,
        
        /// <summary>
        /// Package is not acquired
        /// </summary>
        NotAcquired
    }
}