using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Octopus.Client.Model
{
    [DebuggerDisplay("{ToString()}")]
    public class ScopeSpecification : Dictionary<ScopeField, ScopeValue>, IEquatable<ScopeSpecification>
    {
        public ScopeSpecification Clone()
        {
            var copy = new ScopeSpecification();
            foreach (var item in this)
            {
                copy.Add(item.Key, new ScopeValue(item.Value));
            }
            return copy;
        }

        [Obsolete("Rank usage is deprecated. Please see https://octopus.com/docs/deployment-process/variables for variable specificity rules.")]
        public int Rank()
        {
            var score = 0;
            score += Score(ScopeField.Private,      1000000000);
            score += Score(ScopeField.User,         100000000);
            score += Score(ScopeField.Action,       10000000);
            score += Score(ScopeField.Machine,      1000000);
            score += Score(ScopeField.TargetRole,   100000);
            score += Score(ScopeField.Role,         10000);
            score += Score(ScopeField.Tenant,       1001);
            score += Score(ScopeField.TenantTag,    1000);
            score += Score(ScopeField.Environment,  100);
            score += Score(ScopeField.Channel,      10);
            score += Score(ScopeField.Project,      1);
            return score;
        }

        int Score(ScopeField field, int value)
        {
            if (ContainsKey(field) && this[field].Count > 0)
                return value;
            return 0;
        }

        public bool Equals(ScopeSpecification other)
        {
            return other.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Join(" && ", this.Select(p => p.Key + " = (" + string.Join(" || ", p.Value) + ")"));
        }
    }
}