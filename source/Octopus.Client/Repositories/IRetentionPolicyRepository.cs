using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRetentionPolicyRepository
    {
        TaskResource ApplyNow();
    }
}