using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.SqlMaintenance
{
    public class SqlMaintenanceConfigurationResource : IResource
    {
        public SqlMaintenanceConfigurationResource()
        {
            Id = "sqlmaintenance";
        }

        public string Id { get; set; }

        [Writeable]
        public bool IsIndexMaintenanceEnabled { get; set; }

        [Writeable]
        public string IndexMaintenanceSchedule { get; set; } = "0 1 * * SUN";

        [Writeable]
        public bool IsStatisticsUpdateEnabled { get; set; }

        [Writeable]
        public string StatisticsUpdateSchedule { get; set; } = "0 4 * * SUN";

        public LinkCollection Links { get; set; }
    }
}
