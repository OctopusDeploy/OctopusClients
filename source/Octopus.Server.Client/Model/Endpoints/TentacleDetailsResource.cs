using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class TentacleDetailsResource
    {
        [Writeable]
        public bool UpgradeLocked { get; set; }
        public string Version { get; set; }
        public bool UpgradeSuggested { get; set; }
        public bool UpgradeRequired { get; set; }
        public bool UpgradeAvailable { get; set; }
    }
}