using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Git
{
    public class ConvertProjectVariablesToGitCommand
    {
        [Writeable]
        public string CommitMessage { get; set; } = null!;

        [Writeable]
        public string Branch { get; set; } = null!;
    }
}