using System;
using System.Collections.Generic;

namespace Octopus.Client.Model.Accounts.Usages
{
    public class AccountUsageResource : Resource
    {
        public AccountUsageResource()
        {
            Targets = new List<TargetUsageEntry>();
            DeploymentProcesses = new List<StepUsage>();
            Releases = new List<ReleaseUsage>();
            ProjectVariableSets = new List<ProjectVariableSetUsage>();
            LibraryVariableSets = new List<LibraryVariableSetUsageEntry>();
            RunbookProcesses = new List<RunbookStepUsage>();
            RunbookSnapshots = new List<RunbookSnapshotUsage>();
       
        }

        public ICollection<TargetUsageEntry> Targets { get; set; }
        public ICollection<StepUsage> DeploymentProcesses { get; set; }
        public ICollection<ReleaseUsage> Releases { get; set; }
        public ICollection<ProjectVariableSetUsage> ProjectVariableSets { get; set; }
        public ICollection<LibraryVariableSetUsageEntry> LibraryVariableSets { get; set; }
        public ICollection<RunbookStepUsage> RunbookProcesses { get; set; }
        public ICollection<RunbookSnapshotUsage> RunbookSnapshots { get; set; }
    }

    public class StepUsageBase
    {
        public StepUsageBase()
        {
            Steps = new List<StepUsageEntry>();
        }
            
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectSlug { get; set; }
            
        public ICollection<StepUsageEntry> Steps { get; set; }
    }
    
    public class RunbookStepUsage: StepUsageBase
    {
        public string ProcessId { get; set; }
        public string RunbookId { get; set; }
        public string RunbookName { get; set; }
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
            RunbookSnapshots = new List<RunbookSnapshotUsageEntry>();
        }

        public string ProjectSlug { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public ICollection<ReleaseUsageEntry> Releases { get; set; }
        public List<RunbookSnapshotUsageEntry> RunbookSnapshots { get; set; }
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

    public class StepUsage: StepUsageBase { }

    public class StepUsageEntry
    {
        public string StepName { get; set; }
        public string StepId { get; set; }
    }
    
    public class RunbookSnapshotUsage
    {
        public RunbookSnapshotUsage()
        {
            Snapshots = new List<RunbookSnapshotUsageEntry>();
        }

        public string RunbookId { get; set; }
        public string RunbookName { get; set; }
        public string ProjectName { get; set; }
        public string ProjectId { get; set; }
        public ICollection<RunbookSnapshotUsageEntry> Snapshots { get; set; }
    }

    public class RunbookSnapshotUsageEntry
    {
        public string SnapshotId { get; set; }
        public string SnapshotName { get; set; }
    }
}
