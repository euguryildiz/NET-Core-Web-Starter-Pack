using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Utilities.Hash
{
    public static class Encryption 
    {
        public static string SHA1Hash(string text)
        {
            string source = text;
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                return hash;
            }
        }
    }
}

