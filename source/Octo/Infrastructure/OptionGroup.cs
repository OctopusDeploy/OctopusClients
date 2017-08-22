using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Octopus.Cli.Util;

namespace Octopus.Cli.Infrastructure
{
    public class Options
    {
        readonly List<Tuple<string, OptionSet>> options = new List<Tuple<string, OptionSet>>();

        public OptionSet For(string groupName)
        {
            var val = options.FirstOrDefault(o => o.Item1 == groupName);
            if (val == null)
            {
                var o = new OptionSet();
                options.Add(new Tuple<string, OptionSet>(groupName, o));
                return o;
            }
            return val.Item2;
        }

        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (var group in options.ToArray().Reverse())
            {
                o.WriteLine(group.Item1 + ": ");
                o.WriteLine();
                group.Item2.WriteOptionDescriptions(o);
                o.WriteLine();
            }
        }

        public List<string> Parse(IEnumerable<string> arguments)
        {
            var combined = new OptionSet();
            foreach (var group in options)
            {
                combined.AddRange(group.Item2);
            }

            return combined.Parse(arguments);
        }
    }
}
