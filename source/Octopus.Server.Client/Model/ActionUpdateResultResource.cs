using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class ActionUpdateResultResource : IResource
    {
        public ActionUpdateResultResource()
        {
            Links = new LinkCollection();
        }

        public string Id { get; set; }
        public ActionUpdateOutcome Outcome { get; set; }
        public IDictionary<string, string[]> ManualMergeRequiredReasonsByPropertyName { get; set; }
        public string[] NamesOfNewParametersMissingDefaultValue { get; set; }
        public ICollection<ActionUpdateRemovedPackageUsage> RemovedPackageUsages { get; set; } = new List<ActionUpdateRemovedPackageUsage>();
        public LinkCollection Links { get; set; }
    }
}