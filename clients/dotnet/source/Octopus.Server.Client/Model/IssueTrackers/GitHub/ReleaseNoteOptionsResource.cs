#nullable enable
using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.IssueTrackers.GitHub;

public class ReleaseNoteOptionsResource
{
    public const string UsernameDescription = "Set the username to authenticate with against GitHub. Leave blank if using a Personal Access Token for authentication.";
    public const string PasswordDescription = "Set the password or Personal Access Token to authenticate with against GitHub.";
    public const string ReleaseNotePrefixDescription = "Set the prefix to look for when finding release notes for GitHub issues. For example `Release note:`.";

    [DisplayName("Username")]
    [Description(UsernameDescription)]
    [Writeable]
    public string? Username { get; set; }

    [DisplayName("Password")]
    [Description(PasswordDescription)]
    [Writeable]
    public SensitiveValue? Password { get; set; }

    [DisplayName("Release note prefix")]
    [Description(ReleaseNotePrefixDescription)]
    [Writeable]
    public string? ReleaseNotePrefix { get; set; }
}