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
            CommunicationStyle.TentaclePassive => new KubernetesTentacleEndpointResource(
                new ListeningTentacleEndpointConfigurationResource(TentacleThumbprint, GetListeningUri()) { ProxyId = proxyId }),
            CommunicationStyle.TentacleActive => new KubernetesTentacleEndpointResource(
                new PollingTentacleEndpointConfigurationResource(TentacleThumbprint, SubscriptionId.ToString())),
            _ => null
        };
    }
}