using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class InvitationResource : Resource, IHaveSpaceResource
    {
        [Writeable]
        [Required(ErrorMessage = "Please specify which teams the user will be invited to join.")]
        public ReferenceCollection AddToTeamIds { get; set; }
        public string InvitationCode { get; set; }
        public DateTimeOffset Expires { get; set; }
        public string SpaceId { get; set; } 
    }
}