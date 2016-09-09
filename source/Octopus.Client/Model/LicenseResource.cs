using System;

namespace Octopus.Client.Model
{
    public class LicenseResource : Resource
    {
        [Writeable]
        public bool UseFreeVersion { get; set; }

        [Writeable]
        public string LicenseText { get; set; }

        public bool IsCompliant { get; set; }
        public string NoncomplianceReason { get; set; }
    }
}