using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.SshKnownHosts;

public class DeleteSshKnownHostCommand
{
    [Required]
    public string Id { get; set; }
}
