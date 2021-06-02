using System;

namespace Octopus.Client.Model
{
    public enum ActivityStatus
    {
        Pending,
        Running,
        Success,
        Failed,
        Skipped,
        SuccessWithWarning,
        Canceled
    }
}