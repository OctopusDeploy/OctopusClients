using System;

namespace Octopus.Client.Model
{
    public class TaskProgress
    {
        public int ProgressPercentage { get; set; }
        public string EstimatedTimeRemaining { get; set; }
    }
}