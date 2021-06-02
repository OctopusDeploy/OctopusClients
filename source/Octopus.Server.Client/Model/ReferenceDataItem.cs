using System;

namespace Octopus.Client.Model
{
    public class ReferenceDataItem
    {
        public ReferenceDataItem(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}