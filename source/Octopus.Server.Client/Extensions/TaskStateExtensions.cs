using System;
using Octopus.Client.Model;

namespace Octopus.Client.Extensions
{
    public static class TaskStateExtensions
    {
        public static bool IsCompleted(this TaskState state)
        {
            return !(state == TaskState.Executing || state == TaskState.Queued || state == TaskState.Cancelling);
        }
    }
}
