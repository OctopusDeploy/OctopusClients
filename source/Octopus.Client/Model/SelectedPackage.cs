using System;

namespace Octopus.Client.Model
{
    public class SelectedPackage
    {
        public SelectedPackage()
        {
        }

        public SelectedPackage(string stepName, string version)
        {
            StepName = stepName;
            Version = version;
        }

        public string StepName { get; set; }
        public string Version { get; set; }
    }
}