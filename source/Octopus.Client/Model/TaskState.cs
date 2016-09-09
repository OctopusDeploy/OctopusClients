using System;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Represents the different states a task goes through.
    /// </summary>
    public enum TaskState
    {
        Queued = 1,
        Executing = 2,
        Failed = 3,
        Canceled = 4,
        TimedOut = 5,
        Success = 6,
        Cancelling = 8
    }
}