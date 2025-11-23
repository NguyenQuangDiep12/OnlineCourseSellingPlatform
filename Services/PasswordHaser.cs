using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using OnlineCourseSellingPlatform.Interfaces;
using System.Security.Cryptography;

namespace OnlineCourseSellingPlatform.Services
{
    public class PasswordHaser : IPasswordHasher
    {
        public string HashPassword(string Password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string Hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}.{Hashed}";
        }

        public bool VerifyPassword(string Password, string ProvidedPassword)
        {
            var parts = Password.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]); // decode salt
            var hash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
                ));
            return hash == hashed;
        }
    }
}
