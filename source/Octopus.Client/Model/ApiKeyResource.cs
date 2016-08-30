using System;

namespace Octopus.Client.Model
{
    public class ApiKeyResource : Resource
    {
        [WriteableOnCreate]
        public string Purpose { get; set; }

        public string UserId { get; set; }
        public string ApiKey { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}