using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DefectResource : Resource
    {
        [JsonConstructor]
        public DefectResource()
        {
        }

        public DefectResource(string description)
            : this()
        {
            Description = description;
        }

        public DefectResource(string description, DefectStatus status)
            : this()
        {
            Description = description;
            Status = status;
        }

        [Required(ErrorMessage = "Please specify the defect description.")]
        [WriteableOnCreate]
        public string Description { get; set; }

        public DefectStatus? Status { get; set; }
    }
}