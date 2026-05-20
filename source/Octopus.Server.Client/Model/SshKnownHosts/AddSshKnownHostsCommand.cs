using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.SshKnownHosts;

/// <summary>
/// Adds a list of SSH known host entries
/// </summary>
public class AddSshKnownHostsCommand
{
    /// <summary>
    /// A list of SSH known host entries.
    /// <remarks>
    /// Each entry corresponds to an output line from the ssh-keyscan tool or from the known_hosts file.
    /// Each entry must be in the format '{host} {keytype} {publickey}'
    /// </remarks>
    /// </summary>
    [Required]
    public string[] KnownHostEntries { get; set; }
}
