using System;

namespace Octopus.Client.Exceptions
{
    public class IncludeGlobalCannotBeSetFromFalseToTrueException : Exception
    {
        public IncludeGlobalCannotBeSetFromFalseToTrueException(): base("IncludeGlobal cannot be reset from false to true")
        {
        }
    }
}