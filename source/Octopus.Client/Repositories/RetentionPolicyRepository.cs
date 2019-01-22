using System;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IRetentionPolicyRepository
    {
        TaskResource ApplyNow(string spaceId);
    }
    
    class RetentionPolicyRepository : BasicRepository<RetentionPolicyResource>, IRetentionPolicyRepository
    {
        public RetentionPolicyRepository(IOctopusRepository repository)
            : base(repository, "RetentionPolicies")
        {
        }

        public TaskResource ApplyNow(string spaceId = null)
        {
            var tasks = new TaskRepository(Repository);
            var task = new TaskResource { Name = "Retention", Description = "Request to apply retention policies via the API", SpaceId = spaceId};
            return tasks.Create(task);
        }
    }
}