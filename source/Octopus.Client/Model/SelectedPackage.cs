using System;

namespace Octopus.Client.Model
{
    public class SelectedPackage
    {
        public SelectedPackage()
        {
        }

        public SelectedPackage(string actionName, string version)
        {
            ActionName = actionName;
            Version = version;
        }

        [Obsolete("Replaced by " + nameof(ActionName))]
        public string StepName
        {
            get => ActionName;
            set => ActionName = value;
        }

        public string ActionName { get; set; }
        public string Version { get; set; }
    }
}