using Octopus.Client.Model;

namespace OctopusTools.Commands
{
    public class LibraryVariableSetExport
    {
        public LibraryVariableSetResource LibraryVariableSet { get; set; }
        public VariableSetResource VariableSet { get; set; }
    }
}