using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class GetLiveStatusResponse
{
    [Required]
    public IReadOnlyCollection<KubernetesMachineLiveStatusResource> MachineStatuses { get; set; }

    [Required]
    public LiveStatusSummaryResource Summary { get; set; }
}