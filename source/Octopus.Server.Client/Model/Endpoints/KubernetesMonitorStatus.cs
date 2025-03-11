using Octopus.TinyTypes;

namespace Octopus.Client.Model.Endpoints;

public class KubernetesMonitorStatus(string value) : CaseInsensitiveStringTinyType(value)
{
    public static KubernetesMonitorStatus NotRegistered => new("NotRegistered");
    public static KubernetesMonitorStatus Offline => new("Offline");
    public static KubernetesMonitorStatus Online => new("Online");
}