namespace Octopus.Client.Model.Endpoints;

public class PollingKubernetesTentacleEndpointResource : PollingTentacleEndpointResource
{
#pragma warning disable 672
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentaclePassive;
#pragma warning disable 672
}