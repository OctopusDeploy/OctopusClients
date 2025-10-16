using System;
using System.Linq;
using Octopus.TinyTypes;

namespace Octopus.Client.Model
{
    public class TagSetIdOrName : CaseInsensitiveStringTinyType
    {
        public TagSetIdOrName(string value) : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("TagSet ID or name cannot be null or whitespace", nameof(value));
            }

            var tokens = value.Split('/');
            if (tokens.Length != 1)
            {
                throw new ArgumentException("TagSet ID or name must be a single token without '/'", nameof(value));
            }

            if (tokens.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("TagSet ID or name cannot be empty", nameof(value));
            }
        }
    }
}
