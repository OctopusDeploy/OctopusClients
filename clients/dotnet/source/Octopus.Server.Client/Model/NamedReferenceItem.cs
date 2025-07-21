using System;

namespace Octopus.Client.Model
{
    public class NamedReferenceItem
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }

        public bool DisplayIdAndName { get; set; }
    }
}