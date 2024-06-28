using System;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Operations;

public class RegisterKubernetesWorkerOperation : RegisterWorkerOperation, IRegisterKubernetesWorkerOperation
{
    public RegisterKubernetesWorkerOperation() : this(null)
    {
    }

    public RegisterKubernetesWorkerOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
    {
    }
    
    protected override void PrepareWorkerForReRegistration(WorkerResource worker, string proxyId)
    {
        worker.Endpoint = GenerateEndpoint(proxyId);
    }
    
    protected override EndpointResource GenerateEndpoint(string proxyId)
    {
        var endpoint = CommunicationStyle switch
        {
            CommunicationStyle.TentaclePassive => new KubernetesTentacleEndpointResource(
                new ListeningTentacleEndpointConfigurationResource(TentacleThumbprint, GetListeningUri()) { ProxyId = proxyId }),
            CommunicationStyle.TentacleActive => new KubernetesTentacleEndpointResource(
                new PollingTentacleEndpointConfigurationResource(TentacleThumbprint, SubscriptionId.ToString())),
            _ => throw new ArgumentOutOfRangeException(nameof(CommunicationStyle), CommunicationStyle, $"Must be either {CommunicationStyle.TentacleActive} or {CommunicationStyle.TentaclePassive} for this operation")
        };
        
        return endpoint;
    }
}