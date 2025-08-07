using Octopus.Client.Model.Observability.KubernetesMonitor;

namespace Octopus.Client.Repositories;

public interface IKubernetesMonitorRepository
{
    GetKubernetesMonitorByIdResponse Get(GetKubernetesMonitorByIdRequest request);
    RegisterKubernetesMonitorResponse RegisterKubernetesMonitor(RegisterKubernetesMonitorCommand command);
    void Delete(DeleteKubernetesMonitorByIdCommand request);
}

public class KubernetesMonitorRepository(IOctopusRepository repository) : IKubernetesMonitorRepository
{
    public GetKubernetesMonitorByIdResponse Get(GetKubernetesMonitorByIdRequest request)
    {
        var link = repository.Link("KubernetesMonitors");
        return repository.Client.Get<GetKubernetesMonitorByIdResponse>(link, request);
    }

    public RegisterKubernetesMonitorResponse RegisterKubernetesMonitor(RegisterKubernetesMonitorCommand command)
    {
        var link = repository.Link("KubernetesMonitors");
        return repository.Client.Post<RegisterKubernetesMonitorCommand, RegisterKubernetesMonitorResponse>(link, command);
    }

    public void Delete(DeleteKubernetesMonitorByIdCommand request)
    {
        var link = repository.Link("KubernetesMonitors");
        repository.Client.Delete(link, request);
    }
}
