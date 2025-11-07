using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Observability.ContainerLogs;
using Octopus.Client.Model.Observability.KubernetesMonitor;
using Octopus.Client.Model.Observability.LiveStatus;
using Octopus.Client.Model.Observability.ResourceEvents;

namespace Octopus.Client.Repositories.Async;

public interface IObservabilityRepository
{
    Task<GetLiveStatusResponse> Get(GetLiveStatusRequest request, CancellationToken cancellationToken);
    Task<GetResourceResponse> GetLiveKubernetesResource(GetLiveKubernetesResourceRequest request, CancellationToken cancellationToken);

    Task<GetResourceManifestResponse> GetLiveKubernetesResourceManifest(
        GetLiveKubernetesResourceManifestRequest request,
        CancellationToken cancellationToken);

    Task<BeginContainerLogsSessionResponse> BeginContainerLogsSession(
        BeginContainerLogsSessionCommand command,
        CancellationToken cancellationToken);

    Task<GetContainerLogsResponse> GetContainerLogs(
        GetContainerLogsRequest request,
        CancellationToken cancellationToken);

    Task<BeginResourceEventsSessionResponse> BeginResourceEventsSession(
        BeginResourceEventsSessionCommand command,
        CancellationToken cancellationToken);

    Task<GetResourceEventsResponse> GetResourceEvents(
        GetResourceEventsRequest request,
        CancellationToken cancellationToken);
}

public class ObservabilityRepository(IOctopusAsyncRepository repository) : IObservabilityRepository
{
    public async Task<GetLiveStatusResponse> Get(GetLiveStatusRequest request, CancellationToken cancellationToken)
    {
        var link = (string.IsNullOrWhiteSpace(request.TenantId))
            ? await repository.Link("LiveStatus").ConfigureAwait(false)
            : await repository.Link("TenantedLiveStatus").ConfigureAwait(false);

        return await repository.Client.Get<GetLiveStatusResponse>(link, request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<GetResourceResponse> GetLiveKubernetesResource(GetLiveKubernetesResourceRequest request, CancellationToken cancellationToken)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? await repository.Link("KubernetesResource").ConfigureAwait(false)
            : await repository.Link("TenantedKubernetesResource").ConfigureAwait(false);


        return await repository.Client.Get<GetResourceResponse>(link, request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<GetResourceManifestResponse> GetLiveKubernetesResourceManifest(
        GetLiveKubernetesResourceManifestRequest request,
        CancellationToken cancellationToken)
    {
        var link = string.IsNullOrWhiteSpace(request.TenantId)
            ? await repository.Link("KubernetesResourceManifest").ConfigureAwait(false)
            : await repository.Link("TenantedKubernetesResourceManifest").ConfigureAwait(false);

        return await repository.Client.Get<GetResourceManifestResponse>(link, request, cancellationToken);
    }

    public async Task<BeginContainerLogsSessionResponse> BeginContainerLogsSession(
        BeginContainerLogsSessionCommand command,
        CancellationToken cancellationToken)
    {
        var link = await repository.Link("ContainerLogs").ConfigureAwait(false);
        return await repository.Client
            .Post<BeginContainerLogsSessionCommand, BeginContainerLogsSessionResponse>(link, command, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<GetContainerLogsResponse> GetContainerLogs(
        GetContainerLogsRequest request,
        CancellationToken cancellationToken)
    {
        var link = await repository.Link("ContainerLogs").ConfigureAwait(false);
        return await repository.Client.Get<GetContainerLogsResponse>(link, request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<BeginResourceEventsSessionResponse> BeginResourceEventsSession(
        BeginResourceEventsSessionCommand command,
        CancellationToken cancellationToken)
    {
        var link = await repository.Link("ResourceEvents").ConfigureAwait(false);
        return await repository.Client
            .Post<BeginResourceEventsSessionCommand, BeginResourceEventsSessionResponse>(
                link,
                command,
                cancellationToken).ConfigureAwait(false);
    }

    public async Task<GetResourceEventsResponse> GetResourceEvents(
        GetResourceEventsRequest request,
        CancellationToken cancellationToken)
    {
        var link = await repository.Link("ResourceEvents").ConfigureAwait(false);
        return await repository.Client.Get<GetResourceEventsResponse>(link, request, cancellationToken)
            .ConfigureAwait(false);
    }
}
