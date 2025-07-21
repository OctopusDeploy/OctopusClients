#nullable enable
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.IssueTrackers.AzureDevOps;

public class ReleaseNoteOptionsResource
{
    [Writeable]
    public string? ReleaseNotePrefix { get; set; }
}