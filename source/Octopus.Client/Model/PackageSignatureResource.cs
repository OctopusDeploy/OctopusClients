namespace Octopus.Client.Model
{
    public class PackageSignatureResource
    {
        public byte[] Signature { get; set; }
        public string BaseVersion { get; set; }
    }
}