using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRetentionPolicyRepository
    {
        TaskResource ApplyNow();
    }
    
    class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
    {
        public RetentionPolicyRepository(IOctopusClient client)
            : base(client, "RetentionPolicies")
        {
        }

        public TaskResource ApplyNow()
        {
            var tasks = new TaskRepository(Client);
            var task = new TaskResource { Name = "Retention", Description = "Request to apply retention policies via the API" };
            return tasks.Create(task);
        }
    }
}