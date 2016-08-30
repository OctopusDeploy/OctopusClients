using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Validation;

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
        [ValidSemanticVersionOrMask(ErrorMessage = "Please enter a valid Semantic Version, for example '1.3', '1.3.1-beta0001' or '1.3.1-alpha.1'.")]
        [Trim]
        [Writeable]
        public string Version { get; set; }
    }
}