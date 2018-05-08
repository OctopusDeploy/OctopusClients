using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Octopus.Client.Model.Accounts.Usages
{
    public class AccountUsageResource : Resource
    {
        public AccountUsageResource()
        {
            Targets = new List<UsageEntry<TargetUsageEntry>>();
            DeploymentProcesses = new List<UsageEntry<StepUsage>>();
            Releases = new List<UsageEntry<ReleaseUsage>>();
            ProjectVariableSets = new List<UsageEntry<ProjectVariableSetUsage>>();
            LibraryVariableSets = new List<UsageEntry<LibraryVariableSetUsageEntry>>();
        }

        public ICollection<UsageEntry<TargetUsageEntry>> Targets { get; set; }
        public ICollection<UsageEntry<StepUsage>> DeploymentProcesses { get; set; }
        public ICollection<UsageEntry<ReleaseUsage>> Releases { get; set; }
        public ICollection<UsageEntry<ProjectVariableSetUsage>> ProjectVariableSets { get; set; }
        public ICollection<UsageEntry<LibraryVariableSetUsageEntry>> LibraryVariableSets { get; set; }
    }

    public class TargetUsageEntry
    {
        public string TargetName { get; set; }
        public string TargetId { get; set; }
    }

    public class ProjectVariableSetUsage
    {
        public ProjectVariableSetUsage()
        {
            Releases = new List<ReleaseUsageEntry>();
        }

        public string ProjectSlug { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public ICollection<ReleaseUsageEntry> Releases { get; set; }
        public bool IsCurrentlyBeingUsedInProject { get; set; }
    }

    public class LibraryVariableSetUsageEntry
    {
        public string LibraryVariableSetId { get; set; }
        public string LibraryVariableSetName { get; set; }
    }

    public class ReleaseUsage
    {
        public ReleaseUsage()
        {
            Releases = new List<ReleaseUsageEntry>();
        }

        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public ICollection<ReleaseUsageEntry> Releases { get; set; }
    }

    public class ReleaseUsageEntry
    {
        public string ReleaseId { get; set; }
        public string ReleaseVersion { get; set; }
    }

    public class StepUsage
    {
        public StepUsage()
        {
            Steps = new List<StepUsageEntry>();
        }

        public string ProjectName { get; set; }
        public string ProjectSlug { get; set; }
        public string ProjectId { get; set; }

        public ICollection<StepUsageEntry> Steps { get; set; }
    }

    public class StepUsageEntry
    {
        public string StepName { get; set; }
        public string StepId { get; set; }
    }
}
