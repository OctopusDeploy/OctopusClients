namespace Octopus.Client.Model.Endpoints;

public class ListeningKubernetesTentacleEndpointResource : ListeningTentacleEndpointResource
{
#pragma warning disable 672
    public override CommunicationStyle CommunicationStyle => CommunicationStyle.KubernetesTentaclePassive;
#pragma warning restore 672
}