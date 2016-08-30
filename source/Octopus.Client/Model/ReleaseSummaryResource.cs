using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model
{
    public class ReleaseSummaryResource : Resource
    {
        public ReleaseSummaryResource()
        {
        }

        public ReleaseSummaryResource(string releaseId, string version)
        {
            Id = releaseId;
            Version = version;
        }

        [Required(ErrorMessage = "Please provide a version number for this release.")]
        [StringLength(349, ErrorMessage = "The version number is too long. Please enter a shorter version number.")]
        [Trim]
        [Writeable]
        public string Version { get; set; }
    }
}