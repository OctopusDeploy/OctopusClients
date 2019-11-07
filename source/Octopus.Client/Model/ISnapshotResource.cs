using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public interface ISnapshotResource : IResource
    {
        DateTimeOffset Assembled { get; set; }
        string ProjectId { get; set; }
        List<string> LibraryVariableSetSnapshotIds { get; set; }
        List<SelectedPackage> SelectedPackages { get; }
        string ProjectVariableSetSnapshotId { get; set; }
    }
}
