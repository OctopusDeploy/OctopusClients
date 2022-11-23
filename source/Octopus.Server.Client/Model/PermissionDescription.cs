using System;

namespace Octopus.Client.Model
{
    public class PermissionDescription
    {
        public string Description { get; set; }
        public string[] SupportedRestrictions { get; set; }
        public bool CanApplyAtSystemLevel { get; set; }
        public bool CanApplyAtSpaceLevel { get; set; }
    }
}