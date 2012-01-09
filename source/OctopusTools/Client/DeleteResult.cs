using System;

namespace OctopusTools.Client
{
    public class DeleteResult
    {
        readonly int statusCode;
        readonly string statusDescription;

        public DeleteResult(int statusCode, string statusDescription)
        {
            this.statusCode = statusCode;
            this.statusDescription = statusDescription;
        }

        public bool Success
        {
            get { return StatusCode >= 200 && StatusCode < 300; }
        }

        public int StatusCode
        {
            get { return statusCode; }
        }

        public string StatusDescription
        {
            get { return statusDescription; }
        }

        public override string ToString()
        {
            return StatusCode + ": " + StatusDescription;
        }
    }
}