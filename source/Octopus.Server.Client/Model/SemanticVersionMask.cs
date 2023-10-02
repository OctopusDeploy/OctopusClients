using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Octopus.Client.Model
{
    public static class SemanticVersionMask
    {
        const string PatternIncrement = "i";
        const string PatternCurrent = "c";
        static readonly Regex FormatRegex = new Regex(
            @"
^
  (?<Major>(\d+|i|c))               # Major: digits or 'i' or 'c' (required)
  \.(?<Minor>(\d+|i|c))             # .Minor: digits or 'i' or 'c' (required)
  (\.(?<Build>(\d+|i|c))){0,1}      # .Build: digits or 'i' or 'c' (optional)
  (\.(?<Revision>(\d+|i|c))){0,1}    # .Revision: digits or 'i' or 'c' (optional) 
  (\-(?<PreRelease>([A-z]|[0-9])+(\.(([A-z]|[0-9])+))*)){0,1} # -PreRelease version (optional)
$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static bool IsMask(string versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString))
                return false;

            var maskMatch = new MaskMatchedVersion(versionString);

            return maskMatch.IsValid && (
                    maskMatch.Major.IsSubstitute ||
                    maskMatch.Minor.IsSubstitute ||
                    maskMatch.Build.IsSubstitute ||
                    maskMatch.Revision.IsSubstitute ||
                    maskMatch.Tag.IsSubstitute
                );
        }

        public static SemanticVersion GetLatestMaskedVersion(string mask, List<SemanticVersion> versions)
        {
            var maskMatch = new MaskMatchedVersion(mask);

            var maskMajor = maskMatch.Major.IsPresent && !maskMatch.Major.IsSubstitute ? int.Parse(maskMatch.Major.Value) : 0;
            var maskMinor = maskMatch.Minor.IsPresent && !maskMatch.Minor.IsSubstitute ? int.Parse(maskMatch.Minor.Value) : 0;
            var maskBuild = maskMatch.Build.IsPresent && !maskMatch.Build.IsSubstitute ? int.Parse(maskMatch.Build.Value) : 0;
            var maskRevision = maskMatch.Revision.IsPresent && !maskMatch.Revision.IsSubstitute ? int.Parse(maskMatch.Revision.Value) : 0;

            return versions.Where(v =>
            {
                if (maskMatch.Major.IsSubstitute)
                    return true;

                if (v.Version.Major != maskMajor)
                    return false;

                if (maskMatch.Minor.IsSubstitute)
                    return true;

                if (v.Version.Minor != maskMinor)
                    return false;

                if (maskMatch.Build.IsSubstitute)
                    return true;

                if (v.Version.Build != maskBuild)
                    return false;

                if (maskMatch.Revision.IsSubstitute)
                    return true;
                
                if (v.Version.Revision != maskRevision)
                    return false;

                return true;
            }).OrderByDescending(o => o).FirstOrDefault();

        }

        public static SemanticVersion ApplyMask(string mask, SemanticVersion currentVersion)
        {
            var match = FormatRegex.Match(mask);
            if (!match.Success)
                return SemanticVersion.Parse(mask);

            return currentVersion == null
                ? GenerateVersionFromMask(new MaskMatchedVersion(mask))
                : GenerateVersionFromCurrent(new MaskMatchedVersion(mask), new MaskMatchedVersion(currentVersion.ToString()));
        }

        static SemanticVersion GenerateVersionFromMask(MaskMatchedVersion mask)
        {
            var result = new StringBuilder();
            result.Append(mask.Major.EvaluateFromMask());
            result.Append(mask.Minor.EvaluateFromMask("."));
            result.Append(mask.Build.EvaluateFromMask("."));
            result.Append(mask.Revision.EvaluateFromMask("."));
            result.Append(mask.Tag.EvaluateFromMask("-"));
            return SemanticVersion.Parse(result.ToString());
        }

        static SemanticVersion GenerateVersionFromCurrent(MaskMatchedVersion mask, MaskMatchedVersion current)
        {
            var result = new StringBuilder();
            result.Append(mask.Major.Substitute(current.Major));
            result.Append(mask.Minor.EvaluateFromCurrent(current.Minor, mask.Major));
            result.Append(mask.Build.EvaluateFromCurrent(current.Build, mask.Minor));
            result.Append(mask.Revision.EvaluateFromCurrent(current.Revision, mask.Build));
            result.Append(mask.Tag.EvaluateFromCurrent(current.Tag, mask.Revision));
            return SemanticVersion.Parse(result.ToString());
        }

        class MaskMatchedVersion
        {
            public MaskMatchedVersion(string version)
            {
                var maskMatch = FormatRegex.Match(version);
                
                IsValid = maskMatch.Success;
                Major = new Component(maskMatch.Groups["Major"]);
                Minor = new Component(maskMatch.Groups["Minor"]);
                Build = new Component(maskMatch.Groups["Build"]);
                Revision = new Component(maskMatch.Groups["Revision"]);
                Tag = new TagComponent(maskMatch.Groups["PreRelease"]);
            }

            public bool IsValid { get; private set; }
            public Component Major { get; private set; }
            public Component Minor { get; private set; }
            public Component Build { get; private set; }
            public Component Revision { get; private set; }
            public TagComponent Tag { get; private set; }
          
            public class Component
            {
                protected readonly Group matchGroup;

                public Component(Group matchGroup)
                {
                    this.matchGroup = matchGroup;
                }

                public bool IsPresent
                {
                    get { return matchGroup.Success; }

                }

                public virtual bool IsSubstitute
                {
                    get
                    {
                        if (!IsPresent)
                            return false;

                        return matchGroup.Value == PatternIncrement || matchGroup.Value == PatternCurrent;
                    }
                }

                public string Value
                {
                    get { return matchGroup.Value; }
                }

                public string EvaluateFromMask(string separator = "")
                {
                    return IsPresent ?
                        string.Format("{0}{1}", separator, IsSubstitute ? "0" : Value) :
                        string.Empty;
                }

                public virtual string EvaluateFromCurrent(Component current, Component prevMaskComponent)
                {
                    if (IsPresent)
                    {
                        if(prevMaskComponent.Value != PatternIncrement)
                            return $".{Substitute(current)}";

                        if(IsSubstitute)
                            return ".0";

                        return $".{Substitute(current)}";
                    }

                    if (current.IsPresent && prevMaskComponent.IsPresent)
                    {
                        return ".0";
                    }

                    return string.Empty;
                }

                public virtual string Substitute(Component current)
                {
                    var currentValue = current.IsPresent ? int.Parse(current.Value) : 0;

                    if (Value == PatternIncrement)
                        return (currentValue + 1).ToString(CultureInfo.InvariantCulture);
                    if (Value == PatternCurrent)
                        return (currentValue).ToString(CultureInfo.InvariantCulture);
                    return Value;
                }
            }

            public class TagComponent : Component
            {
                public TagComponent(Group matchGroup) : base(matchGroup)
                {
                }

                public override bool IsSubstitute
                {
                    get
                    {
                        if (!IsPresent)
                            return false;

                        return matchGroup.Value.EndsWith("." + PatternIncrement) || matchGroup.Value.EndsWith("." + PatternCurrent);
                    }
                }

                public override string EvaluateFromCurrent(Component current, Component prevMaskComponent)
                {
                    if (!IsPresent)
                        return "";

                    return "-" + Substitute(current);
                }

                public override string Substitute(Component current)
                {
                    var identifiers = Value.Split('.');
                    var currentIdentifiers = current.IsPresent ? current.Value.Split('.') : new string[0];
                    var substitutedIdentifiers = new List<string>();

                    for (var i = 0; i < identifiers.Length; i++)
                    {
                        if (i > 0 && identifiers[i-1] == PatternIncrement && IsSubstitute)
                        {
                                substitutedIdentifiers.Add("0");
                                continue;
                        }

                        var currentIdentifierValue = 0;
                        if (currentIdentifiers.Length > i)
                            int.TryParse(currentIdentifiers[i], out currentIdentifierValue);

                        switch (identifiers[i])
                        {
                            case PatternIncrement:
                                substitutedIdentifiers.Add((currentIdentifierValue+1).ToString(CultureInfo.InvariantCulture)); 
                                break;
                            case PatternCurrent:
                                substitutedIdentifiers.Add(currentIdentifierValue.ToString(CultureInfo.InvariantCulture));
                                break;
                            default:
                                substitutedIdentifiers.Add(identifiers[i]);
                                break;
                        }
                    }

                    return string.Join(".", substitutedIdentifiers);
                }
            }

        }
    }
}