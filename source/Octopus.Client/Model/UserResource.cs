using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;
using Newtonsoft.Json;
using Octopus.Client.Validation;

namespace Octopus.Client.Model
{
    public class UserResource : Resource
    {
        [Writeable]
        [Trim]
        [StringLength(64)]
        public string Username { get; set; }

        [Writeable]
        [Trim]
        [Required(ErrorMessage = "Please enter a Display name.")]
        [StringLength(64, ErrorMessage = "Please enter a Display name with less than 64 characters.")]
        public string DisplayName { get; set; }

        [Writeable]
        public bool IsActive { get; set; }

        [WriteableOnCreate]
        public bool IsService { get; set; }

        [Writeable]
        [Trim]
        [StringLength(256)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user resource represents the user who requested it.
        /// </summary>
        public bool IsRequestor { get; set; }

        [Writeable]
        [PasswordComplexity]
        [NotReadable]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }
    }
}