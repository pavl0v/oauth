using System.Security.Cryptography;

namespace ExampleClient
{
    public static class Crypto
    {
        public static byte[] ComputeSha512Hash(byte[] data)
        {
            using (var sha512 = new SHA512CryptoServiceProvider())
            {
                return sha512.ComputeHash(data);
            }
        }
    }
}
