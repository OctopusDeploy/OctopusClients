using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Operations;

public class RegisterKubernetesClusterOperation : RegisterMachineOperation, IRegisterKubernetesClusterOperation
{
    public RegisterKubernetesClusterOperation() : this(null)
    {
    }

    public RegisterKubernetesClusterOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
    {
    }

    protected override EndpointResource GenerateEndpoint(string proxyId)
    {
        return CommunicationStyle switch
        {
            CommunicationStyle.TentaclePassive => new KubernetesAgentEndpointResource(
                new ListeningTentacleEndpointConfigurationResource(TentacleThumbprint, GetListeningUri()) { ProxyId = proxyId }),
            CommunicationStyle.TentacleActive => new KubernetesAgentEndpointResource(
                new PollingTentacleEndpointConfigurationResource(TentacleThumbprint, SubscriptionId.ToString())),
            _ => null
        };
    }
}