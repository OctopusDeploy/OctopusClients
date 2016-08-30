using System;

namespace Octopus.Client.Model
{
    public class EncryptedBytes
    {
        readonly byte[] ciphertext;
        readonly byte[] salt;

        public EncryptedBytes(byte[] ciphertext, byte[] salt)
        {
            if (ciphertext == null) throw new ArgumentNullException("ciphertext");
            if (salt == null) throw new ArgumentNullException("salt");
            this.ciphertext = ciphertext;
            this.salt = salt;
        }

        public byte[] Ciphertext
        {
            get { return ciphertext; }
        }

        public byte[] Salt
        {
            get { return salt; }
        }

        public string ToBase64()
        {
            var cipher64 = Convert.ToBase64String(ciphertext);
            var salt64 = Convert.ToBase64String(salt);

            return cipher64 + "|" + salt64;
        }

        public static EncryptedBytes FromBase64(string base64)
        {
            var parts = base64.Split('|');
            var cipher = Convert.FromBase64String(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            return new EncryptedBytes(cipher, salt);
        }
    }
}