using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public interface ISupportSimpleRunbookRun
    {
        Dictionary<string, string> FormValues { get; set; }
        string EnvironmentId { get; set; }
        string TenantId { get; set; }
    }
}
