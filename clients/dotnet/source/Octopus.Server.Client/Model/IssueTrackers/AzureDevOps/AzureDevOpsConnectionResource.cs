#nullable enable
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.IssueTrackers.AzureDevOps;

public class AzureDevOpsConnectionResource
{
    public string? Id { get; set; }

    [Required]
    [Writeable]
    public string? BaseUrl { get; set; }

    [Writeable]
    public SensitiveValue? PersonalAccessToken { get; set; }

    public ReleaseNoteOptionsResource ReleaseNoteOptions { get; set; } = new ReleaseNoteOptionsResource();
}