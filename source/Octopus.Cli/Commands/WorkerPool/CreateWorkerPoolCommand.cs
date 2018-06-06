using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands.WorkerPool
{
    [Command("create-workerpool", Description = "Creates a pool for workers")]
    public class CreateWorkerPoolCommand : ApiCommand, ISupportFormattedOutput
    {
        WorkerPoolResource pool;

        public CreateWorkerPoolCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("WorkerPool creation");
            options.Add("name=", "The name of the worker pool", v => WorkerPoolName = v);
            options.Add("ignoreIfExists", "If the pool already exists, an error will be returned. Set this flag to ignore the error.", v => IgnoreIfExists = true);
        }

        public string WorkerPoolName { get; set; }
        public bool IgnoreIfExists { get; set; }

        public async Task Request()
        {
            if (string.IsNullOrWhiteSpace(WorkerPoolName)) throw new CommandException("Please specify a worker pool name using the parameter: --name=XYZ");
            
            pool = await Repository.WorkerPools.FindByName(WorkerPoolName).ConfigureAwait(false);
            if (pool != null)
            {
                if (IgnoreIfExists)
                {
                    commandOutputProvider.Information("The worker pool {WorkerPool:l} (ID {Id:l}) already exists", pool.Name, pool.Id);
                    return;
                }

                throw new CommandException("The worker pool " + pool.Name + " (ID " + pool.Id + ") already exists");
            }

            commandOutputProvider.Information("Creating worker pool: {WorkerPool:l}", WorkerPoolName);
            pool = await Repository.WorkerPools.Create(new WorkerPoolResource {Name = WorkerPoolName}).ConfigureAwait(false);
        }
        
        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("WorkerPool created. ID: {Id:l}", pool.Id);
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(new
            {
                pool.Id,
                pool.Name,
            });
        }
    }
}