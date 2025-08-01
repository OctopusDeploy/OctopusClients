using Octopus.Client.Model.Observability;

namespace Octopus.Client.Repositories;

public interface IObservabilityRepository
{
    GetLiveStatusResponse Get(GetLiveStatusRequest request);
    GetResourceResponse GetResource(GetResourceRequest request);
    GetResourceManifestResponse GetResourceManifest(GetResourceManifestRequest request);
    RegisterKubernetesMonitorResponse RegisterKubernetesMonitor(RegisterKubernetesMonitorCommand command);
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

    public GetResourceResponse GetResource(GetResourceRequest request)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? repository.Link("Resource")
            : repository.Link("TenantedResource");

        return repository.Client.Get<GetResourceResponse>(link, request);
    }

    public GetResourceManifestResponse GetResourceManifest(GetResourceManifestRequest request)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? repository.Link("ResourceManifest")
            : repository.Link("TenantedResourceManifest");

        return repository.Client.Get<GetResourceManifestResponse>(link, request);
    }

    public RegisterKubernetesMonitorResponse RegisterKubernetesMonitor(RegisterKubernetesMonitorCommand command)
    {
        var link = repository.Link("KubernetesMonitor");
        return repository.Client.Post<RegisterKubernetesMonitorCommand, RegisterKubernetesMonitorResponse>(link, command);
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