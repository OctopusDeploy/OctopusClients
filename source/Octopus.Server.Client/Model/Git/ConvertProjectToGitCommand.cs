using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Git
{
    public class ConvertProjectToGitCommand
    {
        [Required]
        public string CommitMessage { get; set; }

        [Required]
        public GitPersistenceSettingsResource VersionControlSettings { get; set; }
    }
}