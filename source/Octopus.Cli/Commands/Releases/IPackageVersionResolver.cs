namespace Octopus.Cli.Commands.Releases
{
    public interface IPackageVersionResolver
    {
        void AddFolder(string folderPath);
        void Add(string stepNameAndVersion);
        /// <summary>
        /// Adds a package version to be used with the release
        /// </summary>
        /// <param name="stepName">The name of the step or package</param>
        /// <param name="packageReferenceName">The named package, or null if this is the default (unnamed) package</param>
        /// <param name="packageVersion">The version of the package</param>
        void Add(string stepName, string packageReferenceName, string packageVersion);
        /// <summary>
        /// Adds a package version for the unamed package to be used with the release
        /// </summary>
        /// <param name="stepName">The name of the step or package</param>
        /// <param name="packageVersion">The version of the package</param>
        void Add(string stepName, string packageVersion);
        void Default(string packageVersion);
        /// <summary>
        /// Get the version of a previously defined package
        /// </summary>
        /// <param name="stepName">The name of the step or package</param>
        /// <param name="packageReferenceName">The named package, or null if this is the default (unnamed) package</param>
        /// <param name="packageId">The package ID, used as a secondary check to stepName </param>
        /// <returns>The version assigned to the package</returns>
        string ResolveVersion(string stepName, string packageId, string packageReferenceName);
        /// <summary>
        /// Get the version of a previously defined unnamed package (i.e. package reference null is null)
        /// </summary>
        /// <param name="stepName">The name of the step or package</param>
        /// <param name="packageId">The package ID, used as a secondary check to stepName </param>
        /// <returns>The version assigned to the package</returns>
        string ResolveVersion(string stepName, string packageId);
    }
}