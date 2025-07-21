﻿// This class is based on NuGet's SemanticVersion class from https://github.com/NuGet/NuGet.Client
// NuGet is licensed under the Apache license: https://github.com/NuGet/NuGet.Client/blob/dev/LICENSE.txt

using System;
using System.Collections.Generic;
using System.Linq;

namespace Octopus.Client.Model.Versioning
{
    public partial class StrictSemanticVersion
    {
        /// <summary>
        /// Parses a SemVer string using strict SemVer rules.
        /// </summary>
        public static StrictSemanticVersion Parse(string value)
        {
            StrictSemanticVersion ver = null;
            if (!TryParse(value, out ver))
            {
                throw new ArgumentException($"'{value}' is not a valid version string", nameof(value));
            }

            return ver;
        }

        /// <summary>
        /// Parse a version string
        /// </summary>
        /// <returns>false if the version is not a strict semver</returns>
        public static bool TryParse(string value, out StrictSemanticVersion version)
        {
            version = null;

            if (value != null)
            {
                Version systemVersion = null;

                var sections = ParseSections(value);

                // null indicates the string did not meet the rules
                if (sections != null
                    && Version.TryParse(sections.Item1, out systemVersion))
                {
                    // validate the version string
                    var parts = sections.Item1.Split('.');

                    if (parts.Length != 3)
                    {
                        // versions must be 3 parts
                        return false;
                    }

                    foreach (var part in parts)
                    {
                        if (!IsValidPart(part, false))
                        {
                            // leading zeros are not allowed
                            return false;
                        }
                    }

                    // labels
                    if (sections.Item2 != null
                        && !sections.Item2.All(s => IsValidPart(s, false)))
                    {
                        return false;
                    }

                    // build metadata
                    if (sections.Item3 != null
                        && !IsValid(sections.Item3, true))
                    {
                        return false;
                    }

                    var ver = NormalizeVersionValue(systemVersion);

                    version = new StrictSemanticVersion(version: ver,
                        releaseLabels: sections.Item2,
                        metadata: sections.Item3 ?? string.Empty);

                    return true;
                }
            }

            return false;
        }

        internal static bool IsLetterOrDigitOrDash(char c)
        {
            var x = (int)c;

            // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-"
            return (x >= 48 && x <= 57) || (x >= 65 && x <= 90) || (x >= 97 && x <= 122) || x == 45;
        }

        internal static bool IsValid(string s, bool allowLeadingZeros)
        {
            return s.Split('.').All(p => IsValidPart(p, allowLeadingZeros));
        }

        internal static bool IsValidPart(string s, bool allowLeadingZeros)
        {
            return IsValidPart(s.ToCharArray(), allowLeadingZeros);
        }

        internal static bool IsValidPart(char[] chars, bool allowLeadingZeros)
        {
            var result = true;

            if (chars.Length == 0)
            {
                // empty labels are not allowed
                result = false;
            }

            // 0 is fine, but 00 is not. 
            // 0A counts as an alpha numeric string where zeros are not counted
            if (!allowLeadingZeros
                && chars.Length > 1
                && chars[0] == '0'
                && chars.All(c => Char.IsDigit(c)))
            {
                // no leading zeros in labels allowed
                result = false;
            }
            else
            {
                result &= chars.All(c => IsLetterOrDigitOrDash(c));
            }

            return result;
        }

        /// <summary>
        /// Parse the version string into version/release/build
        /// The goal of this code is to take the most direct and optimized path
        /// to parsing and validating a semver. Regex would be much cleaner, but
        /// due to the number of versions created in NuGet Regex is too slow.
        /// </summary>
        internal static Tuple<string, string[], string> ParseSections(string value)
        {
            string versionString = null;
            string[] releaseLabels = null;
            string buildMetadata = null;

            var dashPos = -1;
            var plusPos = -1;

            var chars = value.ToCharArray();

            var end = false;
            for (var i = 0; i < chars.Length; i++)
            {
                end = (i == chars.Length - 1);

                if (dashPos < 0)
                {
                    if (end
                        || chars[i] == '-'
                        || chars[i] == '+')
                    {
                        var endPos = i + (end ? 1 : 0);
                        versionString = value.Substring(0, endPos);

                        dashPos = i;

                        if (chars[i] == '+')
                        {
                            plusPos = i;
                        }
                    }
                }
                else if (plusPos < 0)
                {
                    if (end || chars[i] == '+')
                    {
                        var start = dashPos + 1;
                        var endPos = i + (end ? 1 : 0);
                        var releaseLabel = value.Substring(start, endPos - start);

                        releaseLabels = releaseLabel.Split('.');

                        plusPos = i;
                    }
                }
                else if (end)
                {
                    var start = plusPos + 1;
                    var endPos = i + (end ? 1 : 0);
                    buildMetadata = value.Substring(start, endPos - start);
                }
            }

            return new Tuple<string, string[], string>(versionString, releaseLabels, buildMetadata);
        }

        internal static Version NormalizeVersionValue(Version version)
        {
            var normalized = version;

            if (version.Build < 0
                || version.Revision < 0)
            {
                normalized = new Version(
                    version.Major,
                    version.Minor,
                    Math.Max(version.Build, 0),
                    Math.Max(version.Revision, 0));
            }

            return normalized;
        }

        private static IEnumerable<string> ParseReleaseLabels(string releaseLabels)
        {
            if (!String.IsNullOrEmpty(releaseLabels))
            {
                return releaseLabels.Split('.');
            }

            return null;
        }
    }
}