namespace Octopus.Cli.Infrastructure
{
    public class CouldNotFindException : CommandException
    {
        public CouldNotFindException(string what)
            : base("Could not find " + what  + "; either it does not exist or you lack permissions to view it.")
        {
        }

        public CouldNotFindException(string what, string quotedName)
            : this(what + " '" + quotedName + "'")
        {
        }

    }
}