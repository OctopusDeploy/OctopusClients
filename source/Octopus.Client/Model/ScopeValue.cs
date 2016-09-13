using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model
{
    public class ScopeValue : ReferenceCollection
    {
        [Obsolete] // TODO: [ObsoleteEx(Message = "For persistence only", TreatAsErrorFromVersion = "3.0")]
        public ScopeValue()
        {
        }

        public ScopeValue(string value)
            : this(new[] {value})
        {
        }

        public ScopeValue(string value, params string[] additionalValues)
            : this(new[] {value}.Union(additionalValues ?? new string[0]))
        {
        }

        public ScopeValue(IEnumerable<string> items) : base(items)
        {
        }

        public static implicit operator ScopeValue(string value)
        {
            return new ScopeValue(value);
        }
    }
}