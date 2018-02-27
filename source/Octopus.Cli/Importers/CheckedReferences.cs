using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;

namespace Octopus.Cli.Importers
{
    public class CheckedReferences<T> where T:Resource
    {
        public IDictionary<string, T> FoundDependencies { get; set; }
        public ICollection<string> MissingDependencyNames { get; set; }

        public List<string> FoundDependencyIds => FoundDependencies.Keys.ToList();

        public IEnumerable<string> MissingDependencyErrors
        {
            get { return MissingDependencyNames.Select(d => string.Format("Could not find {0} '{1}'", typeof (T).Name.Replace("Resource",""), d)); }
        }

        public CheckedReferences()
        {
            FoundDependencies = new Dictionary<string, T>();
            MissingDependencyNames = new List<string>();
        }

        public void Register(string uniqueName, string originalId, T resource)
        {
            if (resource == null)
            {
                MissingDependencyNames.Add(uniqueName);
            }
            else
            {
                if (!FoundDependencies.ContainsKey(originalId))
                    FoundDependencies.Add(originalId, resource);
            }
        }
    }
}