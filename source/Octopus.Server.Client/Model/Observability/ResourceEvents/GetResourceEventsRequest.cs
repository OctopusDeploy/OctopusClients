using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ResourceEvents;

public class GetResourceEventsRequest
{
    [Required]
    public string SessionId { get; set; }
}