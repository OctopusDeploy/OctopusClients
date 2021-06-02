using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.VersionControl
{
    public class ConvertProjectToVersionControlledCommand
    {
        [Required]
        public string CommitMessage { get; set; }

        [Required]
        public VersionControlSettingsResource VersionControlSettings { get; set; }
    }
}