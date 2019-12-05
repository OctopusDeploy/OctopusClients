using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunPublishedRunbookResource : Resource, IHaveSpaceResource
    {
        [Required(ErrorMessage = "Please specify the name of the project to run.")]
        [WriteableOnCreate]
        public string ProjectName { get; set; }

        [Required(ErrorMessage = "Please specify the name of the runbook to run.")]
        [WriteableOnCreate]
        public string RunbookName { get; set; }

        [Required(ErrorMessage = "Please specify the name of the environment to run on.")]
        [WriteableOnCreate]
        public string EnvironmentName { get; set; }

        [WriteableOnCreate]
        public string TenantName { get; set; }

        public string SpaceId { get; set; }
    }
}