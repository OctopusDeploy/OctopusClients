using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.SshKnownHosts;

public class AddSshKnownHostsResponse
{
    [Required]
    public SshKnownHostResource[] AddedResources { get; set; }
}
