using System;

namespace Octopus.Client.Model
{
    public class ReportDeploymentCountOverTimeResource : Resource
    {
        public string ProjectId { get; set; }
        public NumericReportData ReportData { get; set; }
    }
}