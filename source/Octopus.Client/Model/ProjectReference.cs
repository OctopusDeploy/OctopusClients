using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ProjectReference
    {
        public ProjectReference(string projectId, string projectName, string projectSlug, string projectGroupId, string projectGroupName, List<DateTime> timestamps)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectSlug = projectSlug;
            ProjectGroupId = projectGroupId;
            ProjectGroupName = projectGroupName;
            TimeStamps = timestamps;
            Score = 100;
        }

        public string ProjectId { get; }

        public string ProjectName { get; }

        public string ProjectSlug { get; }

        public string ProjectGroupId { get; }

        public string ProjectGroupName { get; }

        public List<DateTime> TimeStamps { get; }

        public int Score { get; set; }
    }
}