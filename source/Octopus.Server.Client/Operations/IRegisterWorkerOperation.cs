using System;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering workers.
    /// </summary>
    public interface IRegisterWorkerOperation : IRegisterMachineOperationBase
    {
        /// <summary>
        /// Gets or sets the worker pools that this machine should be added to.
        /// </summary>
        [Obsolete($"Use the {nameof(WorkerPools)} property as it supports worker pool names, slugs and Ids.")]
        string[] WorkerPoolNames { get; set; }
        
        /// <summary>
        /// Gets or sets the worker pools that this machine should be added to. These can be worker pool names, slugs or Ids
        /// </summary>
        string[] WorkerPools { get; set; }
    }
}