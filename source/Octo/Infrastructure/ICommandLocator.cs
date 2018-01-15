namespace Octopus.Cli.Infrastructure
{
    public interface ICommandLocator
    {
        ICommandMetadata[] List();
        ICommand Find(string name);
    }
}