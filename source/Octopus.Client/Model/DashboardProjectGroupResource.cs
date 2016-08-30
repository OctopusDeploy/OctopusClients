using System;

namespace Octopus.Client.Model
{
    public class DashboardProjectGroupResource : Resource
    {
        public string Name { get; set; }
        public ReferenceCollection EnvironmentIds { get; set; }

        public DashboardProjectGroupResource Copy()
        {
            var copy = (DashboardProjectGroupResource)this.MemberwiseClone();
            copy.EnvironmentIds = new ReferenceCollection(this.EnvironmentIds);
            return copy;
        }
    }
}