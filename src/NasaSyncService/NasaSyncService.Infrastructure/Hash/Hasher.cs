using System.Security.Cryptography;
using System.Text;

namespace NasaSyncService.Infrastructure.Hash
{
    internal class Hasher : IHasher
    {
        /// <summary>
        /// Create hash from input string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Hash string</returns>
        public string ComputeHash(string input)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));
        }
    }
}
