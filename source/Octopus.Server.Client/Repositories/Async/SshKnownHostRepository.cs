using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.SshKnownHosts;

namespace Octopus.Client.Repositories.Async;

public interface ISshKnownHostRepository
{
    Task<GetSshKnownHostsResponse> Get(GetSshKnownHostsRequest request, CancellationToken cancellationToken);

    Task<AddSshKnownHostsResponse> Add(AddSshKnownHostsCommand command, CancellationToken cancellationToken);

    Task<DeleteSshKnownHostResponse> Delete(DeleteSshKnownHostCommand command, CancellationToken cancellationToken);
}

class SshKnownHostRepository(IOctopusAsyncRepository repository) : ISshKnownHostRepository
{
    readonly IOctopusAsyncRepository repository = repository;

    public async Task<GetSshKnownHostsResponse> Get(GetSshKnownHostsRequest request, CancellationToken cancellationToken)
    {
        var link = await repository.Link("SshKnownHosts").ConfigureAwait(false);

        return await repository.Client.Get<GetSshKnownHostsResponse>(link, request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<AddSshKnownHostsResponse> Add(AddSshKnownHostsCommand command, CancellationToken cancellationToken)
    {
        var link = await repository.Link("SshKnownHosts").ConfigureAwait(false);

        return await repository.Client.Post<AddSshKnownHostsCommand, AddSshKnownHostsResponse>(link, command, cancellationToken).ConfigureAwait(false);
    }

    public async Task<DeleteSshKnownHostResponse> Delete(DeleteSshKnownHostCommand command, CancellationToken cancellationToken)
    {
        var link = await repository.Link("SshKnownHosts").ConfigureAwait(false);

        return await repository.Client.Delete<DeleteSshKnownHostCommand, DeleteSshKnownHostResponse>(link, command, cancellationToken).ConfigureAwait(false);
    }
}
