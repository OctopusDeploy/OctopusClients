using Octopus.Client.Model.Observability;
using Octopus.Client.Model.Observability.ContainerLogs;
using Octopus.Client.Model.Observability.KubernetesMonitor;
using Octopus.Client.Model.Observability.LiveStatus;
using Octopus.Client.Model.Observability.ResourceEvents;

namespace Octopus.Client.Repositories;

public interface IObservabilityRepository
{
    GetLiveStatusResponse Get(GetLiveStatusRequest request);
    GetResourceResponse GetLiveKubernetesResource(GetLiveKubernetesResourceRequest request);
    GetResourceManifestResponse GetResourceManifest(GetResourceManifestRequest request);
    BeginContainerLogsSessionResponse BeginContainerLogsSession(BeginContainerLogsSessionCommand command);
    GetContainerLogsResponse GetContainerLogs(GetContainerLogsRequest request);
    BeginResourceEventsSessionResponse BeginResourceEventsSession(BeginResourceEventsSessionCommand command);
    GetResourceEventsResponse GetResourceEvents(GetResourceEventsRequest request);
}

public class ObservabilityRepository(IOctopusRepository repository) : IObservabilityRepository
{
    public GetLiveStatusResponse Get(GetLiveStatusRequest request)
    {
        var link = (string.IsNullOrWhiteSpace(request.TenantId))
            ? repository.Link("LiveStatus")
            : repository.Link("TenantedLiveStatus");

        return repository.Client.Get<GetLiveStatusResponse>(link, request);
    }

    public GetResourceResponse GetLiveKubernetesResource(GetLiveKubernetesResourceRequest request)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? repository.Link("KubernetesResource")
            : repository.Link("TenantedKubernetesResource");


        return repository.Client.Get<GetResourceResponse>(link, request);
    }

    public GetResourceManifestResponse GetResourceManifest(GetResourceManifestRequest request)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? repository.Link("ResourceManifest")
            : repository.Link("TenantedResourceManifest");

        return repository.Client.Get<GetResourceManifestResponse>(link, request);
    }

    public BeginContainerLogsSessionResponse BeginContainerLogsSession(BeginContainerLogsSessionCommand command)
    {
        var link = repository.Link("ContainerLogs");
        return repository.Client.Post<BeginContainerLogsSessionCommand, BeginContainerLogsSessionResponse>(link, command);
    }

    public GetContainerLogsResponse GetContainerLogs(GetContainerLogsRequest request)
    {
        var link = repository.Link("ContainerLogs");
        return repository.Client.Get<GetContainerLogsResponse>(link, request);
    }

    public BeginResourceEventsSessionResponse BeginResourceEventsSession(BeginResourceEventsSessionCommand command)
    {
        var link = repository.Link("ResourceEvents");
        return repository.Client.Post<BeginResourceEventsSessionCommand, BeginResourceEventsSessionResponse>(link, command);
    }

    public GetResourceEventsResponse GetResourceEvents(GetResourceEventsRequest request)
    {
        var link = repository.Link("ResourceEvents");
        return repository.Client.Get<GetResourceEventsResponse>(link, request);
    }
}
