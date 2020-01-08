using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Octopus.Client.Util
{
    static class HashCalculator
    {
        public static string Hash(Stream stream)
        {
            using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
                return HashCalculator.Sanitize(cryptoServiceProvider.ComputeHash(stream));
        }

        public static string Hash(byte[] bytes)
        {
            using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
                return HashCalculator.Sanitize(cryptoServiceProvider.ComputeHash(bytes));
        }

        public static string Hash(string input)
        {
            using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
                return HashCalculator.Sanitize(cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        private static string Sanitize(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}