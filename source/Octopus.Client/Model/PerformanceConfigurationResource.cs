using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public enum DashboardRenderMode
    {
        VirtualizeColumns,
        VirtualizeRowsAndColumns
    };

    public class PerformanceConfigurationResource : Resource
    {
        [Writeable]
        public DashboardRenderMode DefaultDashboardRenderMode { get; set; }
    }
}