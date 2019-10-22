using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ProjectReference
    {
        public ProjectReference(string projectId, List<DateTime> timestamps)
        {
            ProjectId = projectId;
            TimeStamps = timestamps;
            Score = 100;
        }
        public string ProjectId { get; }
        public List<DateTime> TimeStamps { get; }
        public int Score { get; set; }
    }
}