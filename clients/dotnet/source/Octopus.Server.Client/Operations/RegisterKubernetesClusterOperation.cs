using System;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Operations;

[Obsolete($"Use {nameof(RegisterKubernetesDeploymentTargetOperation)} instead.")]
public class RegisterKubernetesClusterOperation : RegisterMachineOperation, IRegisterKubernetesClusterOperation
{
    public RegisterKubernetesClusterOperation() : this(null)
    {
    }

    public RegisterKubernetesClusterOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
    {
    }

    public string DefaultNamespace { get; set; }

    protected override void PrepareMachineForReRegistration(MachineResource machine, string proxyId)
    {
        machine.Endpoint = GenerateEndpoint(proxyId);
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

        endpoint.DefaultNamespace = DefaultNamespace;

        return endpoint;
    }
}