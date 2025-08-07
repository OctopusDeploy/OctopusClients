using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.ResourceEvents;

public class BeginResourceEventsSessionResponse
{
    [Required]
    public string SessionId { get; set; }
}