
using System;

namespace Octopus.Client.Model
{
    public enum DeleteMachinesBehavior
    {
        DoNotDelete,
        DeleteUnavailableMachines
    }

    public class MachineCleanupPolicy
    {
        public DeleteMachinesBehavior DeleteMachinesBehavior { get; set; }
        public TimeSpan DeleteMachinesElapsedTimeSpan { get; set; }
    }
}
