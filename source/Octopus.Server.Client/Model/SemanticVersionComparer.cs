using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class SemanticVersionComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            var semX = SemanticVersion.Parse(x);
            var semY = SemanticVersion.Parse(y);
            return semX.CompareTo(semY);
        }
    }
}