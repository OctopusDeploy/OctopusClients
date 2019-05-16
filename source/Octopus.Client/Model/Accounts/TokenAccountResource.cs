using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    public class TokenAccountResource : AccountResource
    {
        public TokenAccountResource()
        {
            Token = new SensitiveValue();
        }

        public override AccountType AccountType => AccountType.Token;

        [Writeable]
        [Required(ErrorMessage = "Please provide a token.")]
        public SensitiveValue Token { get; set; }
    }
}