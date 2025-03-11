using System;
using System.Security.Cryptography;
using System.Text;

namespace LinearXpert
{
    internal class HashFunction
    {
        public static string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                string base64Hash = Convert.ToBase64String(hash);
                return base64Hash;
            }
        }
    }
}
