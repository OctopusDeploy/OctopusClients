using System;

namespace Octopus.Client.Model
{
    public enum ActionUpdateOutcome
    {
        Success = 0,
        ManualMergeRequired = 1,
        DefaultParamterValueMissing = 2
    }
}