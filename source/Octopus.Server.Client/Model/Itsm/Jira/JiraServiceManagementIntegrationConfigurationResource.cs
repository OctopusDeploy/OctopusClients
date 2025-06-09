using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Itsm.Jira;

[Description("Integrate Jira Service Management Change Requests into your deployments. [Learn more](https://oc.to/JiraServiceManagementIntegration).")]
public class JiraServiceManagementIntegrationConfigurationResource : ExtensionConfigurationResource
{
    public JiraServiceManagementIntegrationConfigurationResource()
    {
        Id = "jiraservicemanagement-integration";
    }
    
    [Description("Connect your Octopus instance to one or more Jira Service Management instances")]
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
    [DisplayName("Connection")]
    [ReadOnly(true)]
    public List<JiraServiceManagementIntegrationConnectionResource> Connections { get; } = new();

    [Description("If enabled, customer visible comments will be added to the linked issue at start/end of deployment.")]
    [DisplayName("Customer Comments Enabled")]
    [Required]
    public bool WorkNotesIsEnabled { get; set; }
}