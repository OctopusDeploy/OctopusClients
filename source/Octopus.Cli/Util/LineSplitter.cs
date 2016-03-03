using System;
using System.Collections.Generic;
using System.Text;

namespace Octopus.Cli.Util
{
    public static class LineSplitter
    {
        const int MaxLineLength = 80;

        public static string Split(string indentation, string text)
        {
            var lines = new List<string>();

            var currentLine = new StringBuilder();
            var lastWasWhitespace = false;
            var additional = indentation.Length;

            var pop = new Action<bool>(delegate(bool isForceWrap)
            {
                var current = currentLine.ToString();
                if (string.IsNullOrWhiteSpace(current)) return;

                var next = indentation;

                if (isForceWrap)
                {
                    var lastSpace = current.LastIndexOf(' ');
                    if (lastSpace > MaxLineLength - additional - 10)
                    {
                        next += current.Substring(lastSpace).Trim();
                        current = current.Substring(0, lastSpace).TrimEnd();
                    }
                }

                lines.Add(current);
                currentLine.Clear();
                currentLine.Append(next);
                additional = 0;
            });

            foreach (var c in text)
            {
                if (c == '\r' || c == '\n')
                {
                    pop(false);
                    lastWasWhitespace = true;
                    continue;
                }

                if (currentLine.Length >= MaxLineLength - additional)
                {
                    pop(true);
                }

                if (char.IsWhiteSpace(c) && lastWasWhitespace)
                {
                    continue;
                }

                currentLine.Append(c);

                lastWasWhitespace = char.IsWhiteSpace(c);
            }

            pop(false);

            return string.Join(Environment.NewLine, lines);
        }
    }
}