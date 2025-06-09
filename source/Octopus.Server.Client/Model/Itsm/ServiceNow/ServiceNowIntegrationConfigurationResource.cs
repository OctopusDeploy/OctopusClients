using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Itsm.ServiceNow;

[Description("Integrate ServiceNow Change Requests into your deployments. [Learn more](https://oc.to/ServiceNowIntegration).</br></br>This feature **requires** an enterprise license")]
public class ServiceNowIntegrationConfigurationResource : ExtensionConfigurationResource
{
    [Description("Connect your Octopus instance to one or more ServiceNow instances")]
    [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
    [DisplayName("Connection")]
    [ReadOnly(true)]
    public List<ServiceNowIntegrationConnectionResource> Connections { get; set; } = new();

    [Description("If enabled, work notes will be added to the linked Change Request.")]
    [DisplayName("Work Notes Enabled")]
    [Required]
    public bool WorkNotesIsEnabled { get; set; }
}