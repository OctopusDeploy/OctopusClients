using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Observability;
using Octopus.Client.Model.Observability.ContainerLogs;
using Octopus.Client.Model.Observability.KubernetesMonitor;
using Octopus.Client.Model.Observability.LiveStatus;
using Octopus.Client.Model.Observability.ResourceEvents;

namespace Octopus.Client.Repositories.Async;

public interface IKubernetesMonitorRepository
{
    Task<GetKubernetesMonitorByIdResponse> Get(GetKubernetesMonitorByIdRequest request, CancellationToken cancellationToken);
    Task<RegisterKubernetesMonitorResponse> RegisterKubernetesMonitor(RegisterKubernetesMonitorCommand command, CancellationToken cancellationToken);
    Task Delete(DeleteKubernetesMonitorByIdCommand request, CancellationToken cancellationToken);
}

public class KubernetesMonitorRepository(IOctopusAsyncRepository repository) : IKubernetesMonitorRepository
{
    public async Task<GetKubernetesMonitorByIdResponse> Get(GetKubernetesMonitorByIdRequest request, CancellationToken cancellationToken)
    {
        var link = await repository.Link("KubernetesMonitors").ConfigureAwait(false);
        return await repository.Client
            .Get<GetKubernetesMonitorByIdResponse>(link, request, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<RegisterKubernetesMonitorResponse> RegisterKubernetesMonitor(
        RegisterKubernetesMonitorCommand command,
        CancellationToken cancellationToken)
    {
        var link = await repository.Link("KubernetesMonitors").ConfigureAwait(false);
        return await repository.Client
            .Post<RegisterKubernetesMonitorCommand, RegisterKubernetesMonitorResponse>(link, command, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task Delete(DeleteKubernetesMonitorByIdCommand request, CancellationToken cancellationToken)
    {
        var link = await repository.Link("KubernetesMonitors").ConfigureAwait(false);
        await repository.Client.Delete(link, request, cancellationToken);
    }
}
