using System;

namespace Octopus.Client.Model
{
    public class RegisterCommand : UserResource
    {
        [Writeable]
        public string InvitationCode { get; set; }
    }
}