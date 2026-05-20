using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.SshKnownHosts;

public class GetSshKnownHostsResponse
{
    [Required]
    public SshKnownHostResource[] Resources { get; set; }
    
    [Required]
    public int TotalCount { get; set; }
    
    public int FilteredCount { get; set; }
}
