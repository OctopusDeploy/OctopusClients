using System.Collections.Generic;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.IssueTrackers.AzureDevOps
{
    public class AzureDevOpsConfigurationResource : ExtensionConfigurationResource
    {
        public AzureDevOpsConfigurationResource()
        {
            Id = "issuetracker-azuredevops-v2";
            Connections = new List<AzureDevOpsConnectionResource>();
        }
        
        public IList<AzureDevOpsConnectionResource> Connections { get; }
    }
}