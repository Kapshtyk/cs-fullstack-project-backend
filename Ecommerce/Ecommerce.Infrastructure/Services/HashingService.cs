using System.Security.Cryptography;
using Ecommerce.Services.Common.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Ecommerce.Infrastructure.Services
{
    public class HashingService : IHashingService
    {
        public byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(16);
        }

        public void HashValue(string originalValue, out string hashedValue, byte[] salt)
        {
            hashedValue = Convert.ToBase64String(
                KeyDerivation.Pbkdf2
                (
                    password: originalValue,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32
                )
            );
        }

        public bool VerifyValue(string inputValue, string storedValue, byte[] salt)
        {
            var hashOriginal = Convert.ToBase64String(
                KeyDerivation.Pbkdf2
                (
                   password: inputValue,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 32
                )
            );

            return hashOriginal == storedValue;
        }
    }
}