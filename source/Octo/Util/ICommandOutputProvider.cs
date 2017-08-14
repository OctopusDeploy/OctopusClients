namespace Octopus.Cli.Util
{
    public interface ICommandOutputProvider
    {
        void PrintHeader();

        void PrintError();

    }
}