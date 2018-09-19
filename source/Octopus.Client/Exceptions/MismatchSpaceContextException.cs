using System;

namespace Octopus.Client.Exceptions
{
    public class MismatchSpaceContextException : Exception
    {
        public MismatchSpaceContextException(string s) : base(s)
        {
        }
    }
}