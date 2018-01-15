namespace Octopus.Cli.Commands.Releases
{
    public interface IPackageVersionResolver
    {
        void AddFolder(string folderPath);
        void Add(string stepNameAndVersion);
        void Add(string stepName, string packageVersion);
        void Default(string packageVersion);
        string ResolveVersion(string stepName, string packageId);
    }
}