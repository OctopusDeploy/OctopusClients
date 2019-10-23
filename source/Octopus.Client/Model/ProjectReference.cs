using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public abstract class ProjectReference
    {
        public string ProjectId { get; }
        public IReadOnlyList<DateTime> TimeStamps { get; }
        public int Score { get; }
        public int AccessCount { get; }
    }
}