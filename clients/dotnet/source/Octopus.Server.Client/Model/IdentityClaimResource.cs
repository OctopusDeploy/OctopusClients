namespace Octopus.Client.Model
{
    public class IdentityClaimResource
    {
        public IdentityClaimResource()
        {
        }

        public IdentityClaimResource(string value, bool isIdentifyingClaim)
        {
            this.Value = value;
            this.IsIdentifyingClaim = isIdentifyingClaim;
        }

        public string Value { get; set; }

        public bool IsIdentifyingClaim { get; set; }
    }
}