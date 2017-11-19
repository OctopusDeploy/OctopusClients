using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RegisterCommand : UserResource
    {
        [Writeable]
        public string InvitationCode { get; set; }
    }
}