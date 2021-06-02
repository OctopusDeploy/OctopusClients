using System;

namespace Octopus.Client.Model
{
    public class NumericReportData
    {
        public string[] Labels { get; set; }
        public NumericReportSeries[] Series { get; set; }
    }
}