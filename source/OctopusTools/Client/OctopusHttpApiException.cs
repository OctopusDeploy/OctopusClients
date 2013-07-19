using System;

namespace OctopusTools.Client
{
    public class OctopusHttpApiException : Exception
    {
        public OctopusHttpApiException(string message) : base(message)
        {
            
        }
    }
}