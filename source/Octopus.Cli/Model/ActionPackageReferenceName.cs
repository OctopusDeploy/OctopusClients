using System;
using System.Linq;

namespace Octopus.Cli.Model
{
    internal class ActionPackageReferenceName
    {
        const char Delimiter = ':';

        public ActionPackageReferenceName(string actionName, string packageReferenceName)
        {
            ActionName = actionName;
            PackageReferenceName = packageReferenceName ?? "";
        }

        public ActionPackageReferenceName(string actionName)
            : this(actionName, null)
        { }

        public string ActionName { get; }
        public string PackageReferenceName { get; }

        /// <summary>
        /// Parses the supplied input, which can either be an action name alone, or an action name
        /// and package reference name in a combined string
        /// </summary>
        /// <param name="input">The package reference as a single string</param>
        public static ActionPackageReferenceName Parse(string input)
        {            
            var split = input.Split(Delimiter);
            return new ActionPackageReferenceName(
                split.ElementAtOrDefault(0),
                split.ElementAtOrDefault(1)
            );
        }

        public string ToPackageReferenceNameString() =>
            ActionName + (string.IsNullOrEmpty(PackageReferenceName) ? "" : Delimiter + PackageReferenceName);

        /// <summary>
        /// Performs a case-insensitive comparison
        /// </summary>
        public bool ActionNameMatches(string actionName)
        {
            return ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Performs a case-insensitive comparison between the name of this package-reference and the
        /// supplied name.  Nulls and empty strings are considered equal. 
        /// </summary>
        public bool PackageReferenceNameMatches(string name)
        {
            return PackageReferenceName == "" 
                ? string.IsNullOrEmpty(name) 
                : PackageReferenceName.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}