using System;
using System.Collections.Generic;

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
        public LinkCollection Links { get; set; }
    }
}