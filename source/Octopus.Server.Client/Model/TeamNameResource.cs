using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class TeamNameResource : Resource, INamedResource
    {
        /// <summary>
        /// Gets or sets the name of this team.
        /// </summary>
        [Writeable]
        [Trim]
        public string Name { get; set; }
    }
}
