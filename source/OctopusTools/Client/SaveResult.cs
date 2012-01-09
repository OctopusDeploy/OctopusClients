using System;

namespace OctopusTools.Client
{
    public class SaveResult<TResult>
    {
        readonly int statusCode;
        readonly string statusDescription;
        readonly string redirectLocation;
        readonly TResult result;

        public SaveResult(int statusCode, string statusDescription, string redirectLocation, TResult result)
        {
            this.statusCode = statusCode;
            this.statusDescription = statusDescription;
            this.redirectLocation = redirectLocation;
            this.result = result;
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

        public string RedirectLocation
        {
            get { return redirectLocation; }
        }

        public TResult Result
        {
            get { return result; }
        }

        public override string ToString()
        {
            return StatusCode + ": " + StatusDescription + " - returned: " + Result;
        }
    }
}