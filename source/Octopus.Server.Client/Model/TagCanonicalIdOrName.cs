using System;
using System.Linq;
using Octopus.TinyTypes;

namespace Octopus.Client.Model
{
    public class TagCanonicalIdOrName : CaseInsensitiveStringTinyType
    {
        public TagCanonicalIdOrName(string value) : base(value)
        {
            if (!LooksLikeACanonicalIdOrName(value, out var setPart, out var tagPart))
            {
                throw new ArgumentException("Value must look like a canonical tag ID or name");
            }

            TagSetIdOrName = new TagSetIdOrName(setPart);
            TagIdOrName = tagPart;
        }

        public TagSetIdOrName TagSetIdOrName { get; }

        public string TagIdOrName { get; }

        private bool LooksLikeACanonicalIdOrName(string s, out string setPart, out string tagPart)
        {
            setPart = "";
            tagPart = "";

            var tokens = s.Split("/".ToCharArray());
            if (tokens.Length != 2) return false;
            if (tokens.Any(string.IsNullOrWhiteSpace)) return false;

            setPart = tokens[0];
            tagPart = tokens[1];

            return true;
        }
    }
}
