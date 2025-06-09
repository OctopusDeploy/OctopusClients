using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.IssueTrackers.Jira;

public class ReleaseNoteOptionsResource
{
    public const string UsernameDescription = "Set the username to authenticate with against your Jira instance.";
    public const string PasswordDescription = "Set the password to authenticate with against your Jira instance.";
    public const string ReleaseNotePrefixDescription = "Set the prefix to look for when finding release notes for Jira issues. For example `Release note:`.";

    [DisplayName("Jira Username")]
    [Description(UsernameDescription)]
    [Writeable]
    public string Username { get; set; }

    [DisplayName("Jira Password")]
    [Description(PasswordDescription)]
    [Writeable]
    public SensitiveValue Password { get; set; }

    [DisplayName("Release Note Prefix")]
    [Description(ReleaseNotePrefixDescription)]
    [Writeable]
    public string ReleaseNotePrefix { get; set; }
}