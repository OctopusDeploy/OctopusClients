using System;

namespace Octopus.Client.Exceptions
{
    // TODO: Remove me, replace with more specific exceptions
    public class MismatchSpaceContextException : Exception
    {
        public MismatchSpaceContextException(string s) : base(s)
        {
        }
    }
}