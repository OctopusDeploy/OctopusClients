using System;

namespace Octopus.Client.Model
{
    public class DocumentIdFormatException : Exception
    {
        public DocumentIdFormatException(string message)
            : base(message)
        {            
        }
    }
}