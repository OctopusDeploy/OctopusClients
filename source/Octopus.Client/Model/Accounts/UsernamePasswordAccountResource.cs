using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Accounts
{
    public class UsernamePasswordAccountResource : AccountResource
    {
        public UsernamePasswordAccountResource()
        {
            Password = new SensitiveValue();
        }

        public override AccountType AccountType
        {
            get { return AccountType.UsernamePassword; }
        }

        [Trim]
        [Writeable]
        [Required(ErrorMessage = "Please provide a username.")]
        public string Username { get; set; }

        [Writeable]
        public SensitiveValue Password { get; set; }
    }
}