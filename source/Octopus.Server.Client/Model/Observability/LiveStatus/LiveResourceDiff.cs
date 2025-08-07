using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Observability.LiveStatus;

public class LiveResourceDiff
{
    [Required]
    public string Left { get; set; }

    [Required]
    public string Right { get; set; }

    [Required]
    public string Diff { get; set; }
}