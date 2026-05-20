using Octopus.Client.Model;
using Octopus.Client.Model.SshKnownHosts;

namespace Octopus.Client.Repositories;

public interface ISshKnownHostRepository
{
    GetSshKnownHostsResponse Get(GetSshKnownHostsRequest request);

    AddSshKnownHostsResponse Add(AddSshKnownHostsCommand command);

    DeleteSshKnownHostResponse Delete(DeleteSshKnownHostCommand command);
}

class SshKnownHostRepository(IOctopusRepository repository) : ISshKnownHostRepository
{
    readonly IOctopusRepository repository = repository;

    public GetSshKnownHostsResponse Get(GetSshKnownHostsRequest request)
    {
        var link = repository.Link("SshKnownHosts");

        return repository.Client.Get<GetSshKnownHostsResponse>(link, request);
    }

    public AddSshKnownHostsResponse Add(AddSshKnownHostsCommand command)
    {
        var link = repository.Link("SshKnownHosts");

        return repository.Client.Post<AddSshKnownHostsCommand, AddSshKnownHostsResponse>(link, command);
    }

    public DeleteSshKnownHostResponse Delete(DeleteSshKnownHostCommand command)
    {
        var link = repository.Link("SshKnownHosts");

        return repository.Client.Delete<DeleteSshKnownHostCommand, DeleteSshKnownHostResponse>(link, command);
    }
}
