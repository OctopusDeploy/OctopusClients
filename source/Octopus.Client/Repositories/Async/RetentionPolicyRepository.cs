using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRetentionPolicyRepository
    {
        Task<TaskResource> ApplyNow();
    }

    class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
    {
        public RetentionPolicyRepository(IOctopusAsyncClient client)
            : base(client, "RetentionPolicies")
        {
        }

        public Task<TaskResource> ApplyNow()
        {
            var tasks = new TaskRepository(Client);
            var task = new TaskResource { Name = "Retention", Description = "Request to apply retention policies via the API" };
            return tasks.Create(task);
        }
    }
}
