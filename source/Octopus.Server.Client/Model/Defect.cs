using System;

namespace Octopus.Client.Model
{
    public class Defect
    {
        public string Description { get; set; }
        public DefectStatus Status { get; set; }
    }
}