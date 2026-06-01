namespace Octopus.Client.Model.SshKnownHosts;

public class GetSshKnownHostsRequest
{
    public string PartialHost { get; set; }

    public int Skip { get; set; }

    public int Take { get; set; } = 30;
}
