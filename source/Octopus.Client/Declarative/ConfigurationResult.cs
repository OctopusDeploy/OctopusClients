using System.Collections.Generic;

namespace Octopus.Client.Declarative
{
    public class ConfigurationResult
    {
        public ConfigurationResult(IEnumerable<Difference> differences)
        {
            Differences = new List<Difference>(differences);
        }

        public IReadOnlyCollection<Difference> Differences { get; }

        public bool WereDifferencesDetected()
        {
            return Differences.Count > 0;
        }
    }
}