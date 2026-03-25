using System;
using Newtonsoft.Json;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers;

public class WebhookFilterResource : TriggerFilterResource
{
    [JsonConstructor]
    public WebhookFilterResource(Guid? webhookId)
    {
        WebhookId = webhookId;
    }
    public override TriggerFilterType FilterType => TriggerFilterType.WebhookFilter;

    public Guid? WebhookId { get; set; }
}
