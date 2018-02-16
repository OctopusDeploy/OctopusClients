namespace Octopus.Cli.Infrastructure
{
    public interface ICommandMetadata
    {
        string Name { get; }
        string[] Aliases { get; }
        string Description { get; }
    }
}