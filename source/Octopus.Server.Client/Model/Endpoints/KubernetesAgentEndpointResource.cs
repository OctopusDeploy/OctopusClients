using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints;

public class KubernetesAgentEndpointResource : EndpointResource
{
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesAgent;

    public TentacleEndpointConfiguration TentacleEndpointConfiguration { get; set; }

    public DeploymentActionContainerResource DefaultJobExecutionContainer { get; set; }
}

public abstract class TentacleEndpointConfiguration : ITentacleEndpointResource
{
    protected TentacleEndpointConfiguration()
    {
    }

    protected TentacleEndpointConfiguration(string uri, string thumbprint)
    {
        Uri = uri;
        Thumbprint = thumbprint;
    }


    public abstract AgentCommunicationStyleResource CommunicationStyleResource { get; }

    [Trim]
    [Writeable]
    public string Uri { get; set; }

    [Required(ErrorMessage = "Please provide a thumbprint for this machine.")]
    [Trim]
    [Writeable]
    public string Thumbprint { get; set; }

    [Writeable]
    public TentacleDetailsResource TentacleVersionDetails { get; set; }

    public string CertificateSignatureAlgorithm { get; set; }
}

public class PollingTentacleEndpointConfiguration : TentacleEndpointConfiguration, IPollingTentacleEndpointResource
{
    public override AgentCommunicationStyleResource CommunicationStyleResource => AgentCommunicationStyleResource.Polling;

    protected PollingTentacleEndpointConfiguration()
    {
    }

    public PollingTentacleEndpointConfiguration(string uri, string thumbprint) : base(uri, thumbprint)
    {
    }
}

public class ListeningTentacleEndpointConfiguration : TentacleEndpointConfiguration, IListeningTentacleEndpointResource
{
    protected ListeningTentacleEndpointConfiguration()
    {
    }

    public ListeningTentacleEndpointConfiguration(string uri, string thumbprint) : base(uri, thumbprint)
    {
    }

    public override AgentCommunicationStyleResource CommunicationStyleResource => AgentCommunicationStyleResource.Listening;

    [Writeable]
    public string ProxyId { get; set; }
}