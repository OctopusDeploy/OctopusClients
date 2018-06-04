using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

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
        string[] WorkerPoolNames { get; set; }
    }
}