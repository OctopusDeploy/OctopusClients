using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRetentionPolicyRepository
    {
        Task<TaskResource> ApplyNow();
    }
}