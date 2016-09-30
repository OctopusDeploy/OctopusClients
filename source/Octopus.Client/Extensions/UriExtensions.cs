using System;

namespace Octopus.Client.Extensions
{
    public static class UriExtensions
    {
        private static readonly char[] HexUpperChars = {
                                   '0', '1', '2', '3', '4', '5', '6', '7',
                                   '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
                                   };

        // Copied from .NET 4.5 source as it does not exist in NETStandard1.6
        public static string HexEscape(char character)
        {
            if (character > '\xff')
            {
                throw new ArgumentOutOfRangeException("character");
            }
            char[] chars = new char[3];
            int pos = 0;
            EscapeAsciiChar(character, chars, ref pos);
            return new string(chars);
        }

        private static void EscapeAsciiChar(char ch, char[] to, ref int pos)
        {
            to[pos++] = '%';
            to[pos++] = HexUpperChars[(ch & 0xf0) >> 4];
            to[pos++] = HexUpperChars[ch & 0xf];
        }
    }
}